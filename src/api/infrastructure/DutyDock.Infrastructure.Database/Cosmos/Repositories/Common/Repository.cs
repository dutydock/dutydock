using DutyDock.Application.Common.Database.Common;
using DutyDock.Domain.Common.Models.Entities;
using DutyDock.Infrastructure.Database.Common;
using DutyDock.Infrastructure.Database.Common.Specifications;
using DutyDock.Infrastructure.Database.Cosmos.Data;
using Microsoft.Azure.Cosmos;

namespace DutyDock.Infrastructure.Database.Cosmos.Repositories.Common;

internal abstract class Repository<TEntity> where TEntity : Entity
{
    protected readonly DataContainerContext Context;

    private readonly Container _dataContainer;

    protected Repository(DataContainerContext context, ContainerProvider containerProvider)
    {
        Context = context;

        _dataContainer = containerProvider.DataContainer;
    }

    public virtual void Create(TEntity entity)
    {
        var typeName = entity.GetType().GetTypeName();

        var dataObject = new DataObject<TEntity>(
            entity.Id,
            GetPartitionKey(entity),
            typeName,
            entity,
            state: EntityState.Created);

        Context.Add(dataObject);
    }

    public virtual void Update(TEntity entity, string? etag = null)
    {
        var typeName = entity.GetType().GetTypeName();

        var dataObject = new DataObject<TEntity>(
            entity.Id,
            GetPartitionKey(entity),
            typeName,
            entity,
            state: EntityState.Updated)
        {
            Etag = etag,
            Data = entity
        };

        Context.Add(dataObject);
    }

    public virtual async Task Delete(TEntity entity, CancellationToken cancellationToken = default)
    {
        var requestOptions = new ItemRequestOptions { EnableContentResponseOnWrite = false };

        try
        {
            await _dataContainer.DeleteItemAsync<ItemResponse<DataObject<TEntity>>>(entity.Id,
                new PartitionKey(GetPartitionKey(entity)),
                requestOptions, cancellationToken);
        }
        catch (CosmosException ex)
        {
            throw ex.Evaluate(entity.Id);
        }
    }

    public async Task Delete(string id, CancellationToken cancellationToken = default)
    {
        await DeleteInternal(id, null, null, cancellationToken);
    }

    public async Task Delete(string id, string organizationId, CancellationToken cancellationToken = default)
    {
        await DeleteInternal(id, organizationId, null, cancellationToken);
    }

    public async Task Delete(string id, string organizationId, string owningEntityId,
        CancellationToken cancellationToken = default)
    {
        await DeleteInternal(id, organizationId, owningEntityId, cancellationToken);
    }
    
    public async Task<(TEntity?, string?)> GetById(string id, CancellationToken cancellationToken = default)
    {
        return await GetById(id, false, cancellationToken);
    }

    public async Task<(TEntity?, string?)> GetById(string id, bool includeDeleted,
        CancellationToken cancellationToken = default)
    {
        return await GetById(id, null, null, null, includeDeleted, cancellationToken);
    }

    public async Task<(TEntity?, string?)> GetScopedById(string id, string organizationId,
        CancellationToken cancellationToken = default)
    {
        return await GetScopedById(id, organizationId, false, cancellationToken);
    }

    public async Task<(TEntity?, string?)> GetScopedById(string id, string organizationId,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        return await GetById(id, organizationId, null, null, includeDeleted, cancellationToken);
    }

    public async Task<(TEntity?, string?)> GetScopedAndOwnedById(string id, string organizationId,
        string owningEntityId,
        CancellationToken cancellationToken = default)
    {
        return await GetById(id, organizationId, owningEntityId, null, false, cancellationToken);
    }

    protected async Task<(TEntity?, string?)> GetOneBySpecification(
        Specification<TEntity>? specification,
        string? organizationId = null,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var results = await _dataContainer.GetQueryable<TEntity>(organizationId, includeDeleted: includeDeleted)
            .ExecuteAsync(specification, cancellationToken: cancellationToken);
        var result = results.Entities.FirstOrDefault();

        return result == null ? (null, null) : (result.Data, result.Etag);
    }

    protected async Task<ICollection<(TEntity item, string? eTag)>> GetBySpecification(
        Specification<TEntity> specification,
        string? organizationId = null,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var results = await _dataContainer.GetQueryable<TEntity>(organizationId, includeDeleted: includeDeleted)
            .ExecuteAsync(specification, cancellationToken: cancellationToken);

        return results.Entities.Select(result => (result.Data, result.Etag)).ToList();
    }

    private async Task<(TEntity?, string?)> GetById(
        string id,
        string? organizationId = null,
        string? owningEntityId = null,
        string? etag = null,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var requestOptions = new ItemRequestOptions();

        if (!string.IsNullOrWhiteSpace(etag))
        {
            requestOptions.IfNoneMatchEtag = etag;
        }

        try
        {
            var partitionKey = new PartitionKey(GetPartitionKey(id, organizationId, owningEntityId));

            var result = await _dataContainer.ReadItemAsync<DataObject<TEntity>>(id,
                partitionKey, requestOptions, cancellationToken);

            var dataObject = result.Resource.Data;

            if (typeof(ISoftDeletable).IsAssignableFrom(typeof(TEntity)) && ((ISoftDeletable)dataObject).IsDeleted)
            {
                return includeDeleted ? (dataObject, result.ETag) : (null, null);
            }

            return (dataObject, result.ETag);
        }
        catch (CosmosException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return (null, null);
            }

            throw ex.Evaluate(id, etag);
        }
    }

    private async Task DeleteInternal(string id, string? organizationId, string? owningEntityId, CancellationToken cancellationToken = default)
    {
        var requestOptions = new ItemRequestOptions { EnableContentResponseOnWrite = false };

        try
        {
            await _dataContainer.DeleteItemAsync<ItemResponse<DataObject<TEntity>>>(id,
                new PartitionKey(GetPartitionKey(id, organizationId, owningEntityId)),
                requestOptions, cancellationToken);
        }
        catch (CosmosException ex)
        {
            throw ex.Evaluate(id);
        }
    }
    
    private static string GetPartitionKey(TEntity entity)
    {
        if (entity is not IOrganizationScoped scopedEntity)
        {
            return PartitionHelpers.GetApplicationScopedPartitionKey(entity.Id);
        }

        var organizationId = scopedEntity.OrganizationId;

        if (organizationId == null)
        {
            throw new ArgumentException(nameof(organizationId));
        }

        var type = typeof(TEntity);
        var isOwnedEntity = entity is IOwnedEntity;

        return isOwnedEntity
            ? PartitionHelpers.GetResourceScopedPartitionKey(type, organizationId, entity.GetOwningTypeId())
            : PartitionHelpers.GetOrganizationScopedPartitionKey(type, organizationId);
    }

    private static string GetPartitionKey(string id, string? organizationId = null, string? owningEntityId = null)
    {
        var type = typeof(TEntity);

        if (!typeof(IOrganizationScoped).IsAssignableFrom(type))
        {
            return PartitionHelpers.GetApplicationScopedPartitionKey(id);
        }

        if (organizationId == null)
        {
            throw new ArgumentNullException(nameof(organizationId));
        }

        var isOwnedEntity = typeof(IOwnedEntity).IsAssignableFrom(type);

        if (isOwnedEntity && owningEntityId == null)
        {
            throw new ArgumentNullException(nameof(owningEntityId));
        }

        return isOwnedEntity
            ? PartitionHelpers.GetResourceScopedPartitionKey(type, organizationId, owningEntityId!)
            : PartitionHelpers.GetOrganizationScopedPartitionKey(type, organizationId);
    }
}