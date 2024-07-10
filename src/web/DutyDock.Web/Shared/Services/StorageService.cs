using Blazored.LocalStorage;
using DutyDock.Web.Core.Models.Security;
using DutyDock.Web.Core.Services.Interfaces;

namespace DutyDock.Web.Shared.Services;

public class StorageService : IStorageService
{
    private const string UserKey = "user";
    private const string OrganizationKey = "organization";
    private const string StaySignedInKey = "stay-signed-in";

    private readonly ILocalStorageService _localStorageService;

    public StorageService(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    public async Task<UserProfile?> LoadUserProfile()
    {
        try
        {
            return await LoadItem<UserProfile>(UserKey);
        }
        catch (Exception)
        {
            // Something is wrong, trigger a sign-out
            await SaveUserProfile(null);
            return null;
        }
    }

    public async Task SaveUserProfile(UserProfile? userProfile)
    {
        await SaveItem(UserKey, userProfile);
    }

    public async Task<bool> LoadStaySignedIn()
    {
        return await LoadItem<bool>(StaySignedInKey);
    }

    public async Task SaveStaySignedIn(bool? flag)
    {
        await SaveItem(StaySignedInKey, flag);
    }

    public async Task<OrganizationProfile?> LoadOrganizationProfile()
    {
        try
        {
            return await LoadItem<OrganizationProfile>(OrganizationKey);
        }
        catch (Exception)
        {
            // Something is wrong, trigger a sign-out
            await SaveOrganizationProfile(null);
            return null;
        }
    }

    public async Task SaveOrganizationProfile(OrganizationProfile? organizationProfile)
    {
        await SaveItem(OrganizationKey, organizationProfile);
    }

    private async Task SaveItem<T>(string key, T? item)
    {
        if (item == null)
        {
            await _localStorageService.RemoveItemAsync(key);
        }
        else
        {
            await _localStorageService.SetItemAsync(key, item);
        }
    }

    private async Task<T> LoadItem<T>(string key)
    {
        return await _localStorageService.GetItemAsync<T>(key);
    }
}