using DutyDock.Api.Contracts.Common;
using DutyDock.Web.Shared.Security.AuthorizationHandlers.Base;

namespace DutyDock.Web.Shared.Security.AuthorizationHandlers;

public class IsValidatedAuthorizationHandler : BooleanClaimAuthorizationHandler<IsValidatedRequirement>
{
    public IsValidatedAuthorizationHandler(ILogger<IsValidatedAuthorizationHandler> logger) : base(logger)
    {
    }

    protected override string ClaimType => UserClaims.IsValidated;
}