using DutyDock.Domain.Common.Models.Entities;

namespace DutyDock.Application.Common.Database.Common;

public interface IRepository<TEntity> where TEntity : Entity
{
    void Create(TEntity entity);

    void Update(TEntity entity, string? etag = null);

    Task Delete(TEntity entity, CancellationToken cancellationToken = default);

    Task Delete(string id, CancellationToken cancellationToken = default);

    Task Delete(string id, string organizationId, CancellationToken cancellationToken = default);

    Task Delete(string id, string organizationId, string owningEntityId, CancellationToken cancellationToken = default);

    Task<(TEntity?, string?)> GetById(
        string id,
        CancellationToken cancellationToken = default);

    Task<(TEntity?, string?)> GetById(
        string id,
        bool includeDeleted,
        CancellationToken cancellationToken = default);

    Task<(TEntity?, string?)> GetScopedById(
        string id,
        string organizationId,
        CancellationToken cancellationToken = default);

    Task<(TEntity?, string?)> GetScopedById(
        string id,
        string organizationId,
        bool includeDeleted,
        CancellationToken cancellationToken = default);

    Task<(TEntity?, string?)> GetScopedAndOwnedById(string id, string organizationId,
        string owningEntityId,
        CancellationToken cancellationToken = default);
}