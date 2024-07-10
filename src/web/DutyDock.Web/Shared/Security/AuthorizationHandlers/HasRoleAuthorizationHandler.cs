using DutyDock.Api.Contracts.Common;
using DutyDock.Api.Contracts.Dto.Organizations;
using Microsoft.AspNetCore.Authorization;

namespace DutyDock.Web.Shared.Security.AuthorizationHandlers;

public class HasRoleAuthorizationHandler : AuthorizationHandler<HasRoleRequirement>
{
    private readonly ILogger<HasRoleAuthorizationHandler> _logger;

    public HasRoleAuthorizationHandler(ILogger<HasRoleAuthorizationHandler> logger)
    {
        _logger = logger;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, HasRoleRequirement requirement)
    {
        _logger.LogDebug(
            "Evaluating {User} for role {Role}", context.User.ToClaims(), requirement.Role);

        var roleClaim = context.User.FindFirst(claim => claim.Type == UserClaims.Role);

        if (roleClaim == null)
        {
            return Task.CompletedTask;
        }

        if (!Enum.TryParse<Role>(roleClaim.Value, out var role))
        {
            return Task.CompletedTask;
        }

        if ((int)role >= (int)requirement.Role)
        {
            _logger.LogDebug("Authorization succeeded");
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}