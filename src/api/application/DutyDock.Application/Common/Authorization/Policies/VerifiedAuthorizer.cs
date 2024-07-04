using DutyDock.Application.Common.Authorization.Requirements;

namespace DutyDock.Application.Common.Authorization.Policies;

public abstract class VerifiedAuthorizer<TRequest> : Authorizer<TRequest>
{
    public override void BuildPolicy(TRequest request)
    {
        UseRequirement(new MustHaveUnchangedUserSecurityStampRequirement());
        UseRequirement(new MustBeValidatedRequirement());
    }
}