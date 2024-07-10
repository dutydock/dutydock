using DutyDock.Web.Core.Models.Security;

namespace DutyDock.Web.Core.Services.Interfaces;

public interface IStorageService
{
    Task<UserProfile?> LoadUserProfile();
    
    Task SaveUserProfile(UserProfile? userProfile);
    
    Task<bool> LoadStaySignedIn();

    Task SaveStaySignedIn(bool? flag);
    
    Task<OrganizationProfile?> LoadOrganizationProfile();

    Task SaveOrganizationProfile(OrganizationProfile? organizationProfile);
}