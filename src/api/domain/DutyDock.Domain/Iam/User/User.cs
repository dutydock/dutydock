using DutyDock.Domain.Common.Errors;
using DutyDock.Domain.Common.Models.Entities;
using DutyDock.Domain.Iam.User.Entities;
using DutyDock.Domain.Iam.User.Enums;
using DutyDock.Domain.Iam.User.Events;
using DutyDock.Domain.Iam.User.Services;
using DutyDock.Domain.Iam.User.ValueObjects;
using ErrorOr;
using Newtonsoft.Json;
using TimeZone = DutyDock.Domain.Iam.User.ValueObjects.TimeZone;

namespace DutyDock.Domain.Iam.User;

[TypeName("user")]
public sealed class User : DomainEntity, IAggregateRoot
{
    [JsonProperty] public Name Name { get; private set; } = null!;

    [JsonProperty] public EmailAddress EmailAddress { get; private set; } = null!;

    [JsonProperty] public Password? Password { get; private set; }

    [JsonProperty] public SecurityStamp SecurityStamp { get; private set; } = null!;

    [JsonProperty] public bool IsEmailAddressValidated { get; private set; }

    [JsonProperty] public EmailAddressConfirmationCode? EmailAddressConfirmationCode { get; private set; }

    [JsonProperty("memberships")] private List<Membership> _memberships = new();

    [JsonIgnore] public IEnumerable<Membership> Memberships => _memberships.AsReadOnly();

    [JsonProperty] public Culture Culture { get; private set; } = null!;

    [JsonProperty] public TimeZone TimeZone { get; private set; } = null!;

    [JsonProperty] public Language Language { get; private set; } = null!;

    private User()
    {
    }

    internal User(
        string id,
        EmailAddress emailAddress,
        Name name,
        Password password,
        SecurityStamp securityStamp,
        Culture culture,
        TimeZone timeZone,
        Language language) : base(id)
    {
        EmailAddress = emailAddress;
        Name = name;

        Password = password;
        SecurityStamp = securityStamp;

        Culture = culture;
        TimeZone = timeZone;
        Language = language;

        SetCreated();
    }

    internal User(
        string id,
        EmailAddress emailAddress,
        Name name,
        SecurityStamp securityStamp,
        Culture culture,
        TimeZone timeZone,
        Language language) : base(id)
    {
        EmailAddress = emailAddress;
        Name = name;
        SecurityStamp = securityStamp;

        Culture = culture;
        TimeZone = timeZone;
        Language = language;

        SetCreated();
    }

    public void CreateEmailAddressConfirmationCode()
    {
        EmailAddressConfirmationCode = EmailAddressConfirmationCode.Create();
        ModifiedAt = DateTime.UtcNow;

        AddEvent(new ConfirmationCodeCreatedEvent(Id));
    }

    public void SetEmailAddressValidated()
    {
        IsEmailAddressValidated = true;
        EmailAddressConfirmationCode = null;

        ModifiedAt = DateTime.UtcNow;

        AddEvent(new EmailAddressValidatedEvent(Id));
    }

    public ErrorOr<Updated> SetName(string? fullName)
    {
        var fullNameResult = Name.Create(fullName);

        var errors = fullNameResult.ToErrors();

        if (errors.Count != 0)
        {
            return errors;
        }

        Name = fullNameResult.Value;
        ModifiedAt = DateTime.UtcNow;

        return Result.Updated;
    }

    public ErrorOr<Success> SetOwnership(string organizationId)
    {
        var membership = GetMembership(organizationId);

        if (membership == null)
        {
            membership = Membership.AsOwner(organizationId);
            _memberships.Add(membership);
        }
        else
        {
            var result = membership.GrantOwnership();

            if (result.IsError)
            {
                return result.Errors;
            }
        }

        SetModified();
        return Result.Success;
    }

    public ErrorOr<Success> AddAsUninvitedMember(string organizationId)
    {
        var membership = GetMembership(organizationId);

        if (membership != null)
        {
            return BusinessErrors.User.MembershipExists;
        }

        membership = Membership.AsUninvited(organizationId);
        _memberships.Add(membership);

        SetModified();

        return Result.Success;
    }

    public ErrorOr<Success> AddAsInvitedMember(string inviterId, string organizationId, Role role)
    {
        var membership = GetMembership(organizationId);

        if (membership != null)
        {
            return BusinessErrors.User.MembershipExists;
        }

        membership = Membership.AsInvitee(organizationId, role);
        _memberships.Add(membership);

        AddEvent(new UserInvitedEvent(Id, inviterId, organizationId));
        SetModified();

        return Result.Success;
    }

    public ErrorOr<Success> ReInviteAsMember(string inviterId, string organizationId)
    {
        var membership = GetMembership(organizationId);

        if (membership == null)
        {
            return BusinessErrors.User.MembershipNonexistent;
        }

        if (membership.Status == MembershipStatus.Active)
        {
            return BusinessErrors.User.InvalidMembershipStatus;
        }

        if (membership.Status == MembershipStatus.Uninvited)
        {
            var result = membership.SetInvited();

            if (result.IsError)
            {
                return result.Errors;
            }
        }

        AddEvent(new UserInvitedEvent(Id, inviterId, organizationId));

        return Result.Success;
    }

    public ErrorOr<Success> ChangeMembershipRole(string organizationId, Role role)
    {
        var membership = GetMembership(organizationId);

        if (membership == null)
        {
            return BusinessErrors.User.MembershipNonexistent;
        }

        membership.SetRole(role);
        SetModified();

        return Result.Success;
    }

    public ErrorOr<Success> TerminateMembership(string organizationId)
    {
        var membership = GetMembership(organizationId);

        if (membership == null)
        {
            return Result.Success;
        }

        if (membership.IsOwner)
        {
            return BusinessErrors.User.OwnerMembershipTermination;
        }

        _memberships.Remove(membership);

        if (IsPurgeable())
        {
            SetDeleted();
        }
        else
        {
            SetModified();
        }

        return Result.Success;
    }

    public ErrorOr<Success> AcceptMembershipInvite(string organizationId)
    {
        if (IsMinimalAccount())
        {
            return BusinessErrors.User.MinimalAccount;
        }

        var membership = GetMembership(organizationId);

        if (membership == null)
        {
            return BusinessErrors.User.MembershipInviteUnavailable;
        }

        var acceptInviteResult = membership.AcceptInvite();

        return acceptInviteResult.IsError ? acceptInviteResult : Result.Success;
    }

    public ErrorOr<Success> AcceptMembershipInvite(
        string organizationId, string? fullName, string? password,
        IUserPasswordHasher passwordHasher, string? culture = null, string? timeZone = null)
    {
        if (!IsMinimalAccount())
        {
            return BusinessErrors.User.FullAccount;
        }

        var membership = GetMembership(organizationId);

        if (membership == null)
        {
            return BusinessErrors.User.MembershipInviteUnavailable;
        }

        var toFullAccountResult = ToFullAccount(fullName, password, passwordHasher, culture, timeZone);
        var acceptInviteResult = membership.AcceptInvite();

        var errors = toFullAccountResult.ToErrors()
            .Append(acceptInviteResult);

        if (errors.Count != 0)
        {
            return errors;
        }

        SetEmailAddressValidated();

        return Result.Success;
    }

    public ErrorOr<Updated> SetCulture(string? culture)
    {
        var cultureResult = Culture.Create(culture);
        var errors = cultureResult.ToErrors();

        if (errors.Count != 0)
        {
            return errors;
        }

        Culture = cultureResult.Value;
        SetModified();

        return Result.Updated;
    }

    public ErrorOr<Updated> SetTimeZone(string? timeZone)
    {
        var timeZoneResult = TimeZone.Create(timeZone);
        var errors = timeZoneResult.ToErrors();

        if (errors.Count != 0)
        {
            return errors;
        }

        TimeZone = timeZoneResult.Value;
        SetModified();

        return Result.Updated;
    }

    public ErrorOr<Updated> SetLanguage(string? language)
    {
        var languageResult = Language.Create(language);
        var errors = languageResult.ToErrors();

        if (errors.Count != 0)
        {
            return errors;
        }

        Language = languageResult.Value;
        SetModified();

        return Result.Updated;
    }

    public ErrorOr<Updated> SetPassword(string? password, IUserPasswordHasher passwordHasher)
    {
        var passwordResult = Password.Create(password, passwordHasher);
        var errors = passwordResult.ToErrors();

        if (errors.Count != 0)
        {
            return errors;
        }

        Password = passwordResult.Value;
        SecurityStamp = SecurityStamp.Create();

        SetModified();

        return Result.Updated;
    }

    public ErrorOr<Success> SetDefaultOrganization(string organizationId)
    {
        var membership = GetMembership(organizationId);

        if (membership == null)
        {
            return Error.NotFound();
        }

        var result = membership.SetDefault();

        if (result.IsError)
        {
            return result.Errors;
        }

        foreach (var membershipToClear in _memberships.Where(m => m.OrganizationId != organizationId))
        {
            membershipToClear.ClearDefault();
        }

        return Result.Success;
    }

    public Membership? GetDefaultMembership()
    {
        var defaultMembership = _memberships.FirstOrDefault(m => m.IsDefault);

        if (defaultMembership != null)
        {
            return defaultMembership;
        }

        var firstMembership = _memberships.FirstOrDefault();

        if (firstMembership != null && firstMembership.Status == MembershipStatus.Active)
        {
            return firstMembership;
        }

        return null;
    }

    public Membership? GetActiveMembership(string? organizationId)
    {
        return organizationId == null
            ? null
            : _memberships.FirstOrDefault(
                m => m.OrganizationId == organizationId && m.Status == MembershipStatus.Active);
    }

    public Membership? GetMembership(string? organizationId)
    {
        return organizationId == null ? null : _memberships.FirstOrDefault(m => m.OrganizationId == organizationId);
    }

    public Role? GetMembershipRole(string? organizationId)
    {
        var membership = GetMembership(organizationId);
        return membership?.Role;
    }

    public override void SetDeleted()
    {
        base.SetDeleted();

        AddEvent(new UserDeletedEvent(Id));
    }

    public override string ToString()
    {
        return $"{Name.Value} ({Id})";
    }

    public bool IsMinimalAccount()
    {
        return Password == null;
    }

    public ErrorOr<Success> ToFullAccount(
        string? fullName,
        string? password,
        IUserPasswordHasher passwordHasher,
        string? culture = null,
        string? timeZone = null,
        string? language = null)
    {
        if (!IsMinimalAccount())
        {
            return BusinessErrors.User.FullAccount;
        }

        var setNameResult = SetName(fullName);
        var setPasswordResult = SetPassword(password, passwordHasher);
        var setCultureResult = SetCulture(culture);
        var setTimeZoneResult = SetTimeZone(timeZone);
        var setLanguageResult = SetLanguage(language);

        var errors = setNameResult.ToErrors()
            .Append(setPasswordResult)
            .Append(setCultureResult)
            .Append(setTimeZoneResult)
            .Append(setLanguageResult);

        if (errors.Count != 0)
        {
            return errors;
        }

        SetModified();

        return Result.Success;
    }

    /**
     * A user is defined as purgeable when no memberships remain
     * and there has never been a password set (minimal profile, user added by other user).
     */
    private bool IsPurgeable()
    {
        return _memberships.Count == 0 && IsMinimalAccount();
    }
}