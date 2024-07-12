namespace DutyDock.Domain.Common.Models.Entities;

/// <summary>
/// Marker interface for entities with the same lifecycle of an owning entity.
/// </summary>
/// <remarks>
/// This marker interface is to be used with the OwningEntity attribute, which holds
/// the 'foreign key' of the owning entity. This mechanism exists for performance and
/// storage reasons. In an ideal world the owned entities would be stored as a collection
/// on the root aggregate, but because the collection is unbounded and could be large they
/// are stored as separate root aggregates. The marker interface indicates the relation
/// between both, and the value of this attribute will be used by the persistence layer for
/// sharding.
/// </remarks>
public interface IOwnedEntity
{
}