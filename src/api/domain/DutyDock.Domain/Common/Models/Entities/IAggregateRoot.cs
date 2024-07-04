namespace DutyDock.Domain.Common.Models.Entities;

/// <summary>
/// Marker interface for an aggregate root entity.
/// </summary>
/// <remarks>
/// Aggregate roots define a consistency boundary. These
/// root entities and all child entities will always be loaded from
/// and written to the database as a whole.
/// </remarks>
public interface IAggregateRoot
{
}