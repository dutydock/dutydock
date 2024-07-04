namespace DutyDock.Application.Common.Authorization.Policies;

/**
 * User must not be authenticated and as such no authorization requirements.
 */
public abstract class AnonymousAuthorizer<TRequest> : Authorizer<TRequest>
{
    public override void BuildPolicy(TRequest request)
    {
        // No requirements - marker authorizer
    }
}