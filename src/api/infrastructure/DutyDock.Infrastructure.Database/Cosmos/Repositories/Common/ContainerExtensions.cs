using DutyDock.Domain.Common.Models.Entities;
using DutyDock.Infrastructure.Database.Common;
using DutyDock.Infrastructure.Database.Cosmos.Data;
using Microsoft.Azure.Cosmos;

namespace DutyDock.Infrastructure.Database.Cosmos.Repositories.Common;

public static class ContainerExtensions
{
    private static readonly CosmosLinqSerializerOptions LinqSerializerOptions = new()
    {
        PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
    };

    public static IQueryable<DataObject<TEntity>> GetQueryable<TEntity>(this Container container,
        string? organizationId = null,
        string? owningEntityId = null,
        bool includeDeleted = false, 
        string? continuationToken = null,
        int maxItemCount = -1) where TEntity : Entity
    {
        var queryable = GetRawQueryable<TEntity>(container, continuationToken, maxItemCount);
        var type = typeof(TEntity);

        // Aggregate root
        if (typeof(IAggregateRoot).IsAssignableFrom(type))
        {
            var typeName = type.GetTypeName();
            queryable = queryable.Where(dataObject => dataObject.Type == typeName);
        }

        // Soft deletable
        if (typeof(ISoftDeletable).IsAssignableFrom(type) && !includeDeleted)
        {
            queryable = queryable.Where(dataObject => !((ISoftDeletable)dataObject.Data).IsDeleted);
        }

        // Organization scoped
        if (typeof(IOrganizationScoped).IsAssignableFrom(type))
        {
            if (organizationId == null)
            {
                throw new ArgumentNullException(nameof(organizationId));
            }

            queryable = queryable.Where(dataObject => ((IOrganizationScoped)dataObject.Data).OrganizationId == organizationId);
        }

        // Owned entity
        if (typeof(IOwnedEntity).IsAssignableFrom(type))
        {
            if (organizationId == null)
            {
                throw new ArgumentNullException(nameof(organizationId));
            }
            
            if (owningEntityId == null)
            {
                throw new ArgumentNullException(nameof(owningEntityId));
            }

            var partitionKey = PartitionHelpers.GetResourceScopedPartitionKey(type, organizationId, owningEntityId);
            queryable = queryable.Where(dataObject => dataObject.PartitionKey == partitionKey);
        }

        return queryable;
    }

    private static IQueryable<DataObject<TEntity>> GetRawQueryable<TEntity>(Container container, 
        string? continuationToken, int maxItemCount) where TEntity : Entity
    {
        var queryRequestOptions = new QueryRequestOptions
        {
            MaxItemCount = maxItemCount
        };

        if (string.IsNullOrEmpty(continuationToken))
        {
            continuationToken = null;
        }

        return container.GetItemLinqQueryable<DataObject<TEntity>>(
            continuationToken: continuationToken,
            requestOptions: queryRequestOptions,
            linqSerializerOptions: LinqSerializerOptions);
    }
}