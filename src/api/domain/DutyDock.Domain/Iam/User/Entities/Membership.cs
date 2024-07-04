using DutyDock.Domain.Common.Errors;
using DutyDock.Domain.Common.Models.Entities;
using DutyDock.Domain.Iam.User.Enums;
using DutyDock.Domain.Iam.User.ValueObjects;
using ErrorOr;
using Newtonsoft.Json;

namespace DutyDock.Domain.Iam.User.Entities;

public class Membership : Entity
{
    [JsonProperty] public string OrganizationId { get; private set; } = null!;

    [JsonProperty] public Role Role { get; private set; }

    [JsonProperty] public bool IsOwner { get; private set; }

    [JsonProperty] public MembershipStatus Status { get; private set; }

    [JsonProperty] public bool IsDefault { get; private set; }

    [JsonProperty] public DateTime CreatedAt { get; private set; }

    [JsonProperty] public SecurityStamp SecurityStamp { get; private set; } = null!;

    private Membership()
    {
    }

    private Membership(
        string organizationId,
        Role role,
        bool isOwner,
        MembershipStatus status,
        DateTime createdAt,
        SecurityStamp securityStamp)
    {
        OrganizationId = organizationId;
        Role = role;
        IsOwner = isOwner;
        Status = status;
        CreatedAt = createdAt;
        SecurityStamp = securityStamp;
    }

    public static Membership AsOwner(string organizationId)
    {
        return new Membership(
            organizationId, Role.Admin, true, MembershipStatus.Active, DateTime.UtcNow, SecurityStamp.Create());
    }

    public static Membership AsInvitee(string organizationId, Role role)
    {
        return new Membership(
            organizationId, role, false, MembershipStatus.Invited, DateTime.UtcNow, SecurityStamp.Create());
    }

    public static Membership AsUninvited(string organizationId)
    {
        return new Membership(
            organizationId, Role.User, false, MembershipStatus.Uninvited, DateTime.UtcNow, SecurityStamp.Create());
    }

    public ErrorOr<Success> SetInvited()
    {
        if (Status != MembershipStatus.Uninvited)
        {
            return BusinessErrors.User.InvalidMembershipStatus;
        }

        Status = MembershipStatus.Invited;
        return Result.Success;
    }

    public ErrorOr<Success> AcceptInvite()
    {
        if (Status != MembershipStatus.Invited)
        {
            return BusinessErrors.User.InvalidMembershipStatus;
        }

        Status = MembershipStatus.Active;
        return Result.Success;
    }

    public ErrorOr<Success> GrantOwnership()
    {
        if (Status != MembershipStatus.Active)
        {
            return BusinessErrors.User.InvalidMembershipStatus;
        }

        if (Role != Role.Admin)
        {
            return BusinessErrors.User.InvalidMembershipRole;
        }

        if (IsOwner)
        {
            return Result.Success; // No change
        }

        IsOwner = true;
        SecurityStamp = SecurityStamp.Create();

        return Result.Success;
    }

    public ErrorOr<Success> RevokeOwnership()
    {
        if (Status != MembershipStatus.Active)
        {
            return BusinessErrors.User.InvalidMembershipStatus;
        }

        if (!IsOwner)
        {
            return Result.Success; // No change
        }

        IsOwner = false;
        SecurityStamp = SecurityStamp.Create();

        return Result.Success;
    }

    public ErrorOr<Success> SetRole(Role role)
    {
        if (IsOwner && role != Role.Admin)
        {
            return BusinessErrors.User.InvalidMembershipRole;
        }

        if (role == Role)
        {
            return Result.Success; // No change    
        }

        Role = role;
        SecurityStamp = SecurityStamp.Create();

        return Result.Success;
    }

    public ErrorOr<Success> SetDefault()
    {
        if (Status != MembershipStatus.Active)
        {
            return BusinessErrors.User.InvalidMembershipStatus;
        }

        IsDefault = true;
        return Result.Success;
    }

    public void ClearDefault()
    {
        IsDefault = false;
    }
}