using DutyDock.Application.Common.Interfaces.Security;
using ErrorOr;

namespace DutyDock.Application.Common.Authorization.Requirements;

public sealed record MustBeActiveMemberRequirement : IAuthorizationRequirement
{
    public string? OrganizationId { get; set; }
}

public sealed class MustBeActiveMemberRequirementHandler : IAuthorizationHandler<MustBeActiveMemberRequirement>
{
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly ICachedUserProvider _cachedUserProvider;

    public MustBeActiveMemberRequirementHandler(
        ICurrentUserProvider currentUserProvider, 
        ICachedUserProvider cachedUserProvider)
    {
        _currentUserProvider = currentUserProvider;
        _cachedUserProvider = cachedUserProvider;
    }

    public async Task<ErrorOr<Success>> Handle(MustBeActiveMemberRequirement request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.UserId;
        
        // When the requirement provides the organization id to check, use this.
        // Otherwise, fall back to the identity provided organization.
        var organizationId = request.OrganizationId ?? _currentUserProvider.OrganizationId;
        
        var user = await _cachedUserProvider.GetById(userId, cancellationToken);

        var membership = user?.GetActiveMembership(organizationId);

        return membership == null ? AuthorizationErrors.NotActiveMember : Result.Success;
    }
}