namespace DutyDock.Application.Common.Authorization;

public interface IAuthorizer<in T>
{
    IEnumerable<IAuthorizationRequirement> Requirements { get; }
    
    void BuildPolicy(T instance);
}