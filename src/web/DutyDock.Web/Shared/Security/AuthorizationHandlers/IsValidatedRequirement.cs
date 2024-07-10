using Microsoft.AspNetCore.Authorization;

namespace DutyDock.Web.Shared.Security.AuthorizationHandlers;

public sealed class IsValidatedRequirement : IAuthorizationRequirement
{
}