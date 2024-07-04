using DutyDock.Domain.Common.Services;
using Newtonsoft.Json;

namespace DutyDock.Domain.Common.Models.Entities;

/// <summary>
/// Base class for entities.
/// </summary>
/// <remarks>
/// Entities are identified by their ID.
/// As a result, two entities of a particular type are identical if the ID is identical.
/// </remarks>
public abstract class Entity : IEquatable<Entity>
{
    [JsonProperty] public string Id { get; }

    protected Entity()
    {
        Id = IdentityProvider.New();
    }
    
    protected Entity(string id)
    {
        Id = id;
    }

    public override bool Equals(object? obj)
    {
        return obj is Entity entity && Id.Equals(entity.Id);
    }

    public static bool operator ==(Entity? left, Entity? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Entity? left, Entity? right)
    {
        return !Equals(left, right);
    }

    public bool Equals(Entity? other)
    {
        return Equals((object?)other);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}