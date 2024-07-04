namespace DutyDock.Domain.Common.Models.Entities;

/// <summary>
/// Marker interface for soft deletable entities
/// </summary>
/// <remarks>
/// Soft deletion of entities is preferred as it can be done in
/// constant time. It also provides audit and 'undo' options.
/// An asynchronous process can at a later point in time hard delete
/// any entities which were previously soft deleted.
/// </remarks>
public interface ISoftDeletable
{
    public DateTime? DeletedAt { get; }

    public bool IsDeleted { get; }

    void SetDeleted();
}