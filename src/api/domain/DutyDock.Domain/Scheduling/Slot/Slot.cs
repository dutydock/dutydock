using DutyDock.Domain.Common.Models.Entities;
using DutyDock.Domain.Common.Services;
using DutyDock.Domain.Identity.Organization;
using ErrorOr;
using Newtonsoft.Json;

namespace DutyDock.Domain.Scheduling.Slot;

[TypeName("slot")]
public class Slot : DomainEntity, IAggregateRoot, IOrganizationScoped
{
    [JsonProperty] public string OrganizationId { get; private set; } = null!;

    [JsonProperty] public string RosterId { get; private set; } = null!;

    [JsonProperty] public DateTime FromUtc { get; private set; }

    [JsonProperty] public DateTime TillUtc { get; protected set; }

    private Slot()
    {
    }

    private Slot(
        string id,
        string organizationId,
        string rosterId,
        DateTime fromUtc,
        DateTime tillUtc) : base(id)
    {
        OrganizationId = organizationId;
        RosterId = rosterId;
        FromUtc = fromUtc;
        TillUtc = tillUtc;
    }

    public static ErrorOr<Slot> Create(
        Organization organization, Roster.Roster roster, DateTimeOffset from, DateTimeOffset till)
    {
        var errors = new List<Error>();

        var fromUtc = from.UtcDateTime;
        var tillUtc = till.UtcDateTime;

        if (fromUtc >= tillUtc)
        {
            errors.Add(BusinessErrors.Activity.FromAfterTill);
        }

        if (fromUtc < roster.FromUtc)
        {
            errors.Add(BusinessErrors.Activity.BeforeRosterFrom);
        }

        if (tillUtc > roster.TillUtc)
        {
            errors.Add(BusinessErrors.Activity.AfterRosterTill);
        }

        if (errors.Count != 0)
        {
            return errors;
        }

        var id = IdentityProvider.New();
        return new Slot(id, organization.Id, roster.Id, fromUtc, tillUtc);
    }
}