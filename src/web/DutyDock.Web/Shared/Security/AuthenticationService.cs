using DutyDock.Api.Client.Common;
using DutyDock.Api.Contracts.Dto.Users;
using DutyDock.Api.Web.Client;
using DutyDock.Api.Web.Contracts.Users;
using DutyDock.Web.Core.Models.Security;
using DutyDock.Web.Core.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;

namespace DutyDock.Web.Shared.Security;

public class AuthenticationService : IAuthenticationService
{
    private readonly IStorageService _storageService;
    private readonly UserAuthenticationStateProvider _authenticationStateProvider;
    private readonly IWebApiClient _apiClient;

    public AuthenticationService(
        IStorageService storageService,
        AuthenticationStateProvider authenticationStateProvider,
        IWebApiClient apiClient)
    {
        _storageService = storageService;
        _authenticationStateProvider = (UserAuthenticationStateProvider)authenticationStateProvider;
        _apiClient = apiClient;
    }

    public async Task<UserProfile?> GetUserProfile()
    {
        return await _storageService.LoadUserProfile();
    }

    public async Task<OrganizationProfile?> GetOrganizationProfile()
    {
        return await _storageService.LoadOrganizationProfile();
    }

    public async Task SetUserValidated(string userId)
    {
        var userProfile = await GetUserProfile();

        if (userProfile == null || userProfile.Id != userId)
        {
            return;
        }

        userProfile = userProfile with { IsValidated = true };
        await SetProfile(userProfile);
    }

    public async Task<bool> SignIn(string? emailAddress, string? password, bool staySignedIn)
    {
        var result = await _apiClient.Users.SignIn(new SignInRequest
        {
            EmailAddress = emailAddress,
            Password = password,
            IsPersisted = staySignedIn
        });

        if (!result.IsSuccessWithData())
        {
            return false;
        }

        var authResponse = result.Data!;

        var organizationProfile = OrganizationProfile.Map(authResponse.Organization);
        var userProfile = UserProfile.Map(authResponse);

        await SetProfile(userProfile, organizationProfile);

        return true;
    }

    public async Task SetUserProfile(UserProfile userProfile)
    {
        await _storageService.SaveUserProfile(userProfile);
        _authenticationStateProvider.Notify();
    }

    public async Task SetOrganizationProfile(OrganizationProfile organizationProfile)
    {
        await _storageService.SaveOrganizationProfile(organizationProfile);
        _authenticationStateProvider.Notify();
    }

    public async Task SetProfile(AuthenticationDetails authDetails)
    {
        var organizationProfile = OrganizationProfile.Map(authDetails.Organization);
        var userProfile = UserProfile.Map(authDetails);

        await SetProfile(userProfile, organizationProfile);
    }

    public async Task SetProfile(UserProfile userProfile, OrganizationProfile? organizationProfile = null)
    {
        await _storageService.SaveUserProfile(userProfile);
        await _storageService.SaveOrganizationProfile(organizationProfile);

        _authenticationStateProvider.Notify();
    }

    public async Task<bool> SignOut(bool withRemote = true)
    {
        var isSuccess = true;

        if (withRemote)
        {
            var result = await _apiClient.Users.SignOut();
            isSuccess = result.Success;
        }

        await _storageService.SaveUserProfile(null);
        await _storageService.SaveOrganizationProfile(null);

        _authenticationStateProvider.Notify();

        return isSuccess;
    }
}