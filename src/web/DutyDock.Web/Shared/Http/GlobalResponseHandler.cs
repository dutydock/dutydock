using System.Net;
using DutyDock.Api.Client.Handlers;
using DutyDock.Web.Core;
using DutyDock.Web.Core.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace DutyDock.Web.Shared.Http;

public class GlobalResponseHandler : IResponseHandler
{
    private readonly IAuthenticationService _authService;
    private readonly NavigationManager _navigationManager;

    public GlobalResponseHandler(IAuthenticationService authService, NavigationManager navigationManager)
    {
        _authService = authService;
        _navigationManager = navigationManager;
    }
    
    public async Task Handle(HttpResponseMessage message)
    {
        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (message.StatusCode)
        {
            case HttpStatusCode.Forbidden:
                await _authService.SignOut(false);
                _navigationManager.NavigateTo(Routes.SignIn);
                break;
            case HttpStatusCode.Unauthorized:
                await _authService.SignOut(false);
                _navigationManager.NavigateTo(Routes.SignIn);
                break;
            case HttpStatusCode.ServiceUnavailable:
                _navigationManager.NavigateTo(Routes.Error503);
                break;
        }
    }
}