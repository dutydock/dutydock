using Microsoft.AspNetCore.Authorization;

namespace DutyDock.Web.Shared.Security.AuthorizationHandlers.Base;

public abstract class BooleanClaimAuthorizationHandler<T> : AuthorizationHandler<T> where T : IAuthorizationRequirement
{
    private readonly ILogger _logger;
    
    protected BooleanClaimAuthorizationHandler(ILogger logger)
    {
        _logger = logger;
    }

    protected abstract string ClaimType { get; }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, T requirement)
    {
        _logger.LogDebug(
            "Evaluating {User} for {Claim}", context.User.ToClaims(), ClaimType);
        
        var booleanClaim = context.User.FindFirst(claim => claim.Type == ClaimType);

        if (booleanClaim == null)
        {
            return Task.CompletedTask;
        }

        if (!bool.TryParse(booleanClaim.Value, out var boolean))
        {
            return Task.CompletedTask;
        }

        if (boolean)
        {
            _logger.LogDebug("Authorization succeeded");
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}