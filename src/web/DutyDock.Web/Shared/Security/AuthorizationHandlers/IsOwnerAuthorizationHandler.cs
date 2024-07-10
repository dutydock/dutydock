using DutyDock.Api.Contracts.Common;
using DutyDock.Web.Shared.Security.AuthorizationHandlers.Base;

namespace DutyDock.Web.Shared.Security.AuthorizationHandlers;

public class IsOwnerAuthorizationHandler : BooleanClaimAuthorizationHandler<IsOwnerRequirement>
{
    public IsOwnerAuthorizationHandler(ILogger<IsOwnerAuthorizationHandler> logger) : base(logger)
    {
    }

    protected override string ClaimType => UserClaims.IsOwner;
}