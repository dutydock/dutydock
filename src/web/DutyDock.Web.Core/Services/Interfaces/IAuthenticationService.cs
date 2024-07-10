using DutyDock.Api.Contracts.Dto.Users;
using DutyDock.Web.Core.Models.Security;

namespace DutyDock.Web.Core.Services.Interfaces;

public interface IAuthenticationService
{
    Task<UserProfile?> GetUserProfile();

    Task SetUserProfile(UserProfile userProfile);
    
    Task<OrganizationProfile?> GetOrganizationProfile();

    Task SetOrganizationProfile(OrganizationProfile organizationProfile);
    
    Task SetProfile(AuthenticationDetails authDetails);
    
    Task SetProfile(UserProfile userProfile, OrganizationProfile? organizationProfile = null);
    
    Task<bool> SignIn(string? emailAddress, string? password, bool staySignedIn);

    Task<bool> SignOut(bool withRemote = true);
}