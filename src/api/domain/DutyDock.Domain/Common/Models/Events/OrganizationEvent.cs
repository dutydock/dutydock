using Newtonsoft.Json;

namespace DutyDock.Domain.Common.Models.Events;

public abstract class OrganizationEvent : DomainEvent
{
    [JsonProperty] public string OrganizationId { get; private set; } = null!;

    protected OrganizationEvent()
    {
    }

    protected OrganizationEvent(string organizationId, string action) : base(action)
    {
        OrganizationId = organizationId;
    }
}