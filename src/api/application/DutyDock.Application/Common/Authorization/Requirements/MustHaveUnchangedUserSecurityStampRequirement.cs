using DutyDock.Application.Common.Interfaces.Security;
using ErrorOr;

namespace DutyDock.Application.Common.Authorization.Requirements;

public sealed record MustHaveUnchangedUserSecurityStampRequirement : IAuthorizationRequirement
{
}

public class MustHaveUnchangedUserSecurityStampRequirementHandler :
    IAuthorizationHandler<MustHaveUnchangedUserSecurityStampRequirement>
{
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly ICachedUserProvider _cachedUserProvider;

    public MustHaveUnchangedUserSecurityStampRequirementHandler(
        ICurrentUserProvider currentUserProvider,
        ICachedUserProvider cachedUserProvider)
    {
        _currentUserProvider = currentUserProvider;
        _cachedUserProvider = cachedUserProvider;
    }

    public async Task<ErrorOr<Success>> Handle(MustHaveUnchangedUserSecurityStampRequirement request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.UserId;
        var userSecurityStamp = _currentUserProvider.UserSecurityStamp;

        var user = await _cachedUserProvider.GetById(userId, cancellationToken);

        if (user == null || user.SecurityStamp.Value != userSecurityStamp)
        {
            return AuthorizationErrors.UserSecurityStamp;
        }

        return Result.Success;
    }
}