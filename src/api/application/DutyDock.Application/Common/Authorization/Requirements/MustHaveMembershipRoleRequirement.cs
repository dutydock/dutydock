using DutyDock.Application.Common.Interfaces.Security;
using DutyDock.Domain.Iam.User.Enums;
using ErrorOr;

namespace DutyDock.Application.Common.Authorization.Requirements;

public sealed record MustHaveMembershipRoleRequirement : IAuthorizationRequirement
{
    public Role Role { get; set; }
}

public class MustHaveMembershipRoleRequirementHandler :
    IAuthorizationHandler<MustHaveMembershipRoleRequirement>
{
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly ICachedUserProvider _cachedUserProvider;

    public MustHaveMembershipRoleRequirementHandler(
        ICurrentUserProvider currentUserProvider,
        ICachedUserProvider cachedUserProvider)
    {
        _currentUserProvider = currentUserProvider;
        _cachedUserProvider = cachedUserProvider;
    }

    public async Task<ErrorOr<Success>> Handle(MustHaveMembershipRoleRequirement request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.UserId;
        var organizationId = _currentUserProvider.OrganizationId;

        var user = await _cachedUserProvider.GetById(userId, cancellationToken);

        var membership = user?.GetMembership(organizationId);

        if (membership == null || (int)membership.Role < (int)request.Role)
        {
            return AuthorizationErrors.MembershipRole;
        }

        return Result.Success;
    }
}