using DutyDock.Domain.Common.Errors;
using DutyDock.Domain.Common.Models.Entities;
using DutyDock.Domain.Common.Models.ValueObjects;
using DutyDock.Domain.Common.Services;
using DutyDock.Domain.Identity.Organization.Enums;
using DutyDock.Domain.Identity.Organization.Events;
using ErrorOr;
using Newtonsoft.Json;

namespace DutyDock.Domain.Identity.Organization;

[TypeName("organization")]
public sealed class Organization : DomainEntity, IAggregateRoot
{
    [JsonProperty] public string CreatedBy { get; private set; } = null!;

    [JsonProperty] public Name Name { get; private set; } = null!;

    [JsonProperty] public OrganizationStatus Status { get; private set; }

    [JsonProperty] public string? Error { get; private set; }

    private Organization()
    {
    }

    private Organization(string id, string createdBy, Name name) : base(id)
    {
        CreatedBy = createdBy;
        Name = name;
        Status = OrganizationStatus.Created;

        SetCreated();
    }

    public static ErrorOr<Organization> Create(string? name, string creatorId)
    {
        var nameResult = Name.Create(name);
        var errors = nameResult.ToErrors();

        if (errors.Count != 0)
        {
            return errors;
        }

        var id = IdentityProvider.New();
        var organization = new Organization(id, creatorId, nameResult.Value);

        organization.AddEvent(new OrganizationCreatedEvent(organization.Id));

        return organization;
    }

    public void SetInitialized()
    {
        Status = OrganizationStatus.Initialized;
        SetModified();
    }

    public void SetError(string error)
    {
        Error = error;
        Status = OrganizationStatus.Error;

        SetModified();
    }

    public ErrorOr<Updated> SetName(string? name)
    {
        var nameResult = Name.Create(name);
        var errors = nameResult.ToErrors();

        if (errors.Count != 0)
        {
            return errors;
        }

        Name = nameResult.Value;
        SetModified();

        return Result.Updated;
    }

    public override string ToString()
    {
        return $"{Name.Value} ({Id})";
    }
}