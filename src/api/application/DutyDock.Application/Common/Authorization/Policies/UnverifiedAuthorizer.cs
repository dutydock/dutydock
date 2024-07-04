using DutyDock.Application.Common.Authorization.Requirements;

namespace DutyDock.Application.Common.Authorization.Policies;

/**
 * User must be authenticated, but email address must not be verified yet.
 */
public abstract class UnverifiedAuthorizer<TRequest> : Authorizer<TRequest>
{
    public override void BuildPolicy(TRequest request)
    {
        UseRequirement(new MustHaveUnchangedUserSecurityStampRequirement());
    }
}