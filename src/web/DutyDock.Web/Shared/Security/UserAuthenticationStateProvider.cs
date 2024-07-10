using System.Security.Claims;
using DutyDock.Api.Contracts.Common;
using DutyDock.Web.Core.Models.Security;
using DutyDock.Web.Core.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;

namespace DutyDock.Web.Shared.Security;

public class UserAuthenticationStateProvider : AuthenticationStateProvider
{
    private const string AuthenticationScheme = "Cookies";

    private readonly ClaimsPrincipal _defaultClaimsPrincipal = new(new ClaimsIdentity());
    private readonly IStorageService _storageService;

    private ClaimsPrincipal? _claimsPrincipal;

    public UserAuthenticationStateProvider(IStorageService storageService)
    {
        _storageService = storageService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_claimsPrincipal != null)
        {
            return new AuthenticationState(_claimsPrincipal);
        }

        var userProfile = await _storageService.LoadUserProfile();
        var organizationProfile = await _storageService.LoadOrganizationProfile();

        _claimsPrincipal = userProfile == null
            ? _defaultClaimsPrincipal
            : FromUserProfile(userProfile, organizationProfile);

        return new AuthenticationState(_claimsPrincipal);
    }

    public void Notify()
    {
        _claimsPrincipal = null; // Force reload
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private static ClaimsPrincipal FromUserProfile(
        UserProfile userProfile, OrganizationProfile? organizationProfile = null)
    {
        var claims = new List<Claim>
        {
            new(type: UserClaims.UserId, value: userProfile.Id),
            new(type: UserClaims.Name, value: userProfile.Name),
            new(type: UserClaims.IsValidated, value: userProfile.IsValidated.ToString())
        };

        if (organizationProfile != null)
        {
            claims.Add(new Claim(type: UserClaims.Role, value: organizationProfile.Role.ToString()));
            claims.Add(new Claim(type: UserClaims.IsOwner, value: organizationProfile.IsOwner.ToString()));
        }

        var identity = new ClaimsIdentity(claims, AuthenticationScheme);
        return new ClaimsPrincipal(identity);
    }
}