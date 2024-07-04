using DutyDock.Application.Common.Interfaces.Security;
using ErrorOr;

namespace DutyDock.Application.Common.Authorization.Requirements;

public sealed record MustBeOrganizationOwnerRequirement : IAuthorizationRequirement
{
}

public sealed class MustBeOrganizationOwnerRequirementHandler :
    IAuthorizationHandler<MustBeOrganizationOwnerRequirement>
{
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly ICachedUserProvider _cachedUserProvider;

    public MustBeOrganizationOwnerRequirementHandler(
        ICurrentUserProvider currentUserProvider,
        ICachedUserProvider cachedUserProvider)
    {
        _currentUserProvider = currentUserProvider;
        _cachedUserProvider = cachedUserProvider;
    }

    public async Task<ErrorOr<Success>> Handle(MustBeOrganizationOwnerRequirement request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.UserId;
        var organizationId = _currentUserProvider.OrganizationId;

        var user = await _cachedUserProvider.GetById(userId, cancellationToken);

        var membership = user?.GetMembership(organizationId);

        if (membership == null || !membership.IsOwner)
        {
            return AuthorizationErrors.NotOwner;
        }

        return Result.Success;
    }
}