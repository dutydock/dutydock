using DutyDock.Domain.Common.Models.Entities;

namespace DutyDock.Application.Common.Database.Common;

public class VersionedEntity<TEntity> where TEntity : Entity
{
    public VersionedEntity(TEntity entity, string? version)
    {
        Entity = entity;
        Version = version;
    }

    public TEntity Entity { get; }

    public string? Version { get; }
}