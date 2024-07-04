using DutyDock.Application.Common.Interfaces.Security;
using ErrorOr;

namespace DutyDock.Application.Common.Authorization.Requirements;

public sealed record MustBeValidatedRequirement : IAuthorizationRequirement
{
}

public sealed class MustBeValidatedRequirementHandler :
    IAuthorizationHandler<MustBeValidatedRequirement>
{
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly ICachedUserProvider _cachedUserProvider;

    public MustBeValidatedRequirementHandler(
        ICurrentUserProvider currentUserProvider,
        ICachedUserProvider cachedUserProvider)
    {
        _currentUserProvider = currentUserProvider;
        _cachedUserProvider = cachedUserProvider;
    }

    public async Task<ErrorOr<Success>> Handle(MustBeValidatedRequirement request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.UserId;
        var user = await _cachedUserProvider.GetById(userId, cancellationToken);

        if (user == null || !user.IsEmailAddressValidated)
        {
            return AuthorizationErrors.NotValidated;
        }

        return Result.Success;
    }
}