using DutyDock.Domain.Common.Models.Events;

namespace DutyDock.Domain.Iam.Organization.Events;

public sealed class OrganizationCreatedEvent : OrganizationEvent
{
    public OrganizationCreatedEvent()
    {
    }

    public OrganizationCreatedEvent(string organizationId) :
        base(organizationId, nameof(OrganizationCreatedEvent))
    {
    }
}