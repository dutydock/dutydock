using DutyDock.Domain.Common.Models.Entities;
using DutyDock.Domain.Common.Services;
using Newtonsoft.Json;

namespace DutyDock.Domain.Common.Models.Events;

public abstract class DomainEvent : Entity, IEvent
{
    protected DomainEvent()
    {
    }

    protected DomainEvent(string action) : base(IdentityProvider.New())
    {
        Action = action;
        CreatedAt = DateTime.UtcNow;
    }

    protected DomainEvent(string id, string action) : base(id)
    {
        Action = action;
        CreatedAt = DateTime.UtcNow;
    }

    [JsonProperty] public string Action { get; private set; } = null!;

    [JsonProperty] public DateTime CreatedAt { get; private set; }
}