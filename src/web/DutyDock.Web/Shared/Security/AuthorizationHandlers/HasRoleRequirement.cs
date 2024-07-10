using DutyDock.Api.Contracts.Dto.Organizations;
using Microsoft.AspNetCore.Authorization;

namespace DutyDock.Web.Shared.Security.AuthorizationHandlers;

public sealed class HasRoleRequirement : IAuthorizationRequirement
{
    public Role Role { get; set; }

    public HasRoleRequirement(Role role)
    {
        Role = role;
    }
}