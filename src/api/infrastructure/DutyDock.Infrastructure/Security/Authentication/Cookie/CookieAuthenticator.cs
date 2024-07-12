using System.Security.Claims;
using DutyDock.Api.Contracts.Common;
using DutyDock.Application.Common.Interfaces.Security;
using DutyDock.Application.Common.Interfaces.Services;
using DutyDock.Domain.Identity.User;
using DutyDock.Domain.Identity.User.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Throw;

namespace DutyDock.Infrastructure.Security.Authentication.Cookie;

public class CookieAuthenticator : ICookieAuthenticator
{
    private readonly CookieOptions _options;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CookieAuthenticator(
        IOptions<CookieOptions> options,
        IDateTimeProvider dateTimeProvider,
        IHttpContextAccessor httpContextAccessor)
    {
        _options = options.Value;
        _dateTimeProvider = dateTimeProvider;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task Create(User user, Membership? membership = null, bool isPersistent = false)
    {
        var httpContext = GetHttpContext();

        var properties = new AuthenticationProperties
        {
            IsPersistent = isPersistent,
            AllowRefresh = true,
            ExpiresUtc = isPersistent ? _dateTimeProvider.UtcNow.AddDays(_options.ExpiryInDays) : null
        };

        var userSecurityStamp = user.SecurityStamp.Value;
        userSecurityStamp.ThrowIfNull();

        var claims = new List<Claim>
        {
            new(type: UserClaims.UserId, value: user.Id),
            new(type: UserClaims.UserSecurityStamp, value: userSecurityStamp)
        };

        if (membership != null)
        {
            var membershipSecurityStamp = membership.SecurityStamp.Value;
            membershipSecurityStamp.ThrowIfNull();

            claims.Add(new Claim(type: UserClaims.OrganizationId, value: membership.OrganizationId));
            claims.Add(new Claim(type: UserClaims.MembershipSecurityStamp, value: membershipSecurityStamp));
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
    }

    public async Task Destroy()
    {
        var httpContext = GetHttpContext();
        await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
    
    private HttpContext GetHttpContext()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        httpContext.ThrowIfNull();

        return httpContext;
    }
}