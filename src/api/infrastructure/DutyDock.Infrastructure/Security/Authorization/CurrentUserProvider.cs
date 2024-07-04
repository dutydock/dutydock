using DutyDock.Api.Contracts.Common;
using DutyDock.Application.Common.Exceptions;
using DutyDock.Application.Common.Interfaces.Security;
using Microsoft.AspNetCore.Http;

namespace DutyDock.Infrastructure.Security.Authorization;

public class CurrentUserProvider : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string UserId => GetUserClaimValue(UserClaims.UserId)!;

    public string UserSecurityStamp => GetUserClaimValue(UserClaims.UserSecurityStamp)!;

    public string OrganizationId => GetUserClaimValue(UserClaims.OrganizationId)!;

    public string? TryGetOrganizationId => GetUserClaimValue(UserClaims.OrganizationId, false);

    public string MembershipSecurityStamp => GetUserClaimValue(UserClaims.MembershipSecurityStamp)!;

    private string? GetUserClaimValue(string claimType, bool mustExist = true)
    {
        var value = _httpContextAccessor.HttpContext?
            .User.Claims.FirstOrDefault(claim => claim.Type == claimType)?.Value;

        if (mustExist && value == null)
        {
            throw new UnauthorizedException();
        }

        return value;
    }
}