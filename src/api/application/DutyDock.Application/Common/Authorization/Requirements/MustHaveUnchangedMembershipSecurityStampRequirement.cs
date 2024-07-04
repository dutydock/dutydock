using DutyDock.Application.Common.Interfaces.Security;
using ErrorOr;

namespace DutyDock.Application.Common.Authorization.Requirements;

public sealed record MustHaveUnchangedMembershipSecurityStampRequirement : IAuthorizationRequirement
{
}

public sealed class MustHaveUnchangedMembershipSecurityStampRequirementHandler :
    IAuthorizationHandler<MustHaveUnchangedMembershipSecurityStampRequirement>
{
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly ICachedUserProvider _cachedUserProvider;

    public MustHaveUnchangedMembershipSecurityStampRequirementHandler(
        ICurrentUserProvider currentUserProvider,
        ICachedUserProvider cachedUserProvider)
    {
        _currentUserProvider = currentUserProvider;
        _cachedUserProvider = cachedUserProvider;
    }

    public async Task<ErrorOr<Success>> Handle(
        MustHaveUnchangedMembershipSecurityStampRequirement request, CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.UserId;
        var organizationId = _currentUserProvider.OrganizationId;
        var membershipSecurityStamp = _currentUserProvider.MembershipSecurityStamp;

        var user = await _cachedUserProvider.GetById(userId, cancellationToken);

        var membership = user?.GetMembership(organizationId);

        if (membership == null || membership.SecurityStamp.Value != membershipSecurityStamp)
        {
            return AuthorizationErrors.MembershipSecurityStamp;
        }

        return Result.Success;
    }
}