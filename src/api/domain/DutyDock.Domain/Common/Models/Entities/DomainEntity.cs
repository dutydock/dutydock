using DutyDock.Domain.Common.Models.Events;
using Newtonsoft.Json;

namespace DutyDock.Domain.Common.Models.Entities;

/// <summary>
/// Base class for domain entities.
/// </summary>
/// <remarks>
/// This class adds capabilities which are used by practically each entity in the domain.
/// In particular, the ability to record and emit events as part of processing state changes.
/// </remarks>
public abstract class DomainEntity : Entity, IDomainEventEmitter<DomainEvent>, ISoftDeletable
{
    protected DomainEntity()
    {
    }

    protected DomainEntity(string id) : base(id)
    {
    }
    
    [JsonProperty]
    public DateTime CreatedAt { get; protected set; }
    
    [JsonProperty]
    public DateTime? ModifiedAt { get; protected set; }
    
    [JsonProperty]
    public DateTime? DeletedAt { get; protected set; }
    
    [JsonProperty]
    public bool IsDeleted { get; protected set; }

    protected void SetCreated()
    {
        IsNew = true;
        CreatedAt = DateTime.UtcNow;
    }

    protected void SetModified()
    {
        ModifiedAt = DateTime.UtcNow;
    }

    public virtual void SetDeleted()
    {
        if (IsNew)
        {
            return;
        }

        DeletedAt = DateTime.UtcNow;
        IsDeleted = true;
    }
    
    [JsonIgnore] 
    protected bool IsNew { get; private set; }

    private readonly List<DomainEvent> _events = new();
    
    [JsonIgnore] 
    public IReadOnlyList<DomainEvent> Events => _events.AsReadOnly();

    public void AddEvent(DomainEvent domainEvent)
    {
        _events.Add(domainEvent);
    }

    public void RemoveEvent(DomainEvent domainEvent)
    {
        _events.Remove(domainEvent);
    }

    public void RemoveAllEvents()
    {
        _events.Clear();
    }
}