using DutyDock.Domain.Common.Errors;
using ErrorOr;
using DutyDock.Domain.Common.Models.Entities;
using DutyDock.Domain.Common.Models.ValueObjects;
using DutyDock.Domain.Common.Services;
using DutyDock.Domain.Identity.Organization;
using Newtonsoft.Json;

namespace DutyDock.Domain.Scheduling.Roster;

[TypeName("roster")]
public class Roster : DomainEntity, IAggregateRoot, IOrganizationScoped
{
    [JsonProperty] public string OrganizationId { get; private set; } = null!;

    [JsonProperty] public Name Name { get; private set; } = null!;

    [JsonProperty] public DateTime FromUtc { get; private set; }

    [JsonProperty] public DateTime TillUtc { get; protected set; }

    private Roster()
    {
    }

    private Roster(
        string id,
        string organizationId,
        Name name,
        DateTime fromUtc,
        DateTime tillUtc) : base(id)
    {
        OrganizationId = organizationId;
        Name = name;
        FromUtc = fromUtc;
        TillUtc = tillUtc;

        SetCreated();
    }

    public static ErrorOr<Roster> Create(
        Organization organization, string name, DateTimeOffset from, DateTimeOffset till)
    {
        var nameResult = Name.Create(name);
        var errors = nameResult.ToErrors();

        var fromUtc = from.UtcDateTime;
        var tillUtc = till.UtcDateTime;

        if (fromUtc >= tillUtc)
        {
            errors.Add(BusinessErrors.Roster.FromAfterTill);
        }

        if (errors.Count != 0)
        {
            return errors;
        }

        var id = IdentityProvider.New();
        return new Roster(id, organization.Id, nameResult.Value, fromUtc, tillUtc);
    }
}