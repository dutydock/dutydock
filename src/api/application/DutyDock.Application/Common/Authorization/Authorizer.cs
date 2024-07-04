namespace DutyDock.Application.Common.Authorization;

public abstract class Authorizer<TRequest> : IAuthorizer<TRequest>
{
    private readonly HashSet<IAuthorizationRequirement> _requirements = new();

    public IEnumerable<IAuthorizationRequirement> Requirements => _requirements;

    protected void UseRequirement(IAuthorizationRequirement requirement)
    {
        _requirements.Add(requirement);
    }

    public abstract void BuildPolicy(TRequest request);
}