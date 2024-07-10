using DutyDock.Web.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace DutyDock.Web.Shared.Security;

public class SecurityService : ISecurityService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly IAuthorizationService _authorizationService;

    public SecurityService(
        AuthenticationStateProvider authenticationStateProvider,
        IAuthorizationService authorizationService)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _authorizationService = authorizationService;
    }

    public async Task<bool> UserCompliesWith(string policy)
    {
        var authenticationState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authenticationState.User;

        var result = await _authorizationService.AuthorizeAsync(user, policy);
        return result.Succeeded;
    }
}