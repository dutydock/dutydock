using DutyDock.Api.Contracts.Dto.Organizations;
using DutyDock.Api.Contracts.Dto.Users;
using DutyDock.Domain.Identity.Organization;
using DutyDock.Domain.Identity.User;
using DutyDock.Domain.Identity.User.Entities;

namespace DutyDock.Application.Identity.Users.Common;

public static class UserDtoAssembler
{
    public static AuthenticationDetails ToAuthenticationDetails(
        this User user,
        Organization? organization = null,
        Membership? membership = null,
        string? token = null)
    {
        var authenticationDetails = new AuthenticationDetails
        {
            Id = user.Id,
            Name = user.Name.Value,
            IsValidated = user.IsEmailAddressValidated,
            Culture = user.Culture.Value,
            TimeZone = user.TimeZone.Value,
            Language = user.Language.Value
        };

        if (organization != null)
        {
            authenticationDetails.Organization = organization.ToOrganizationInfo(membership);
        }

        return authenticationDetails;
    }

    private static OrganizationInfo ToOrganizationInfo(this Organization organization, Membership? membership = null)
    {
        var organizationInfo = new OrganizationInfo
        {
            Id = organization.Id,
            Name = organization.Name.Value,
            Status = (OrganizationStatus)organization.Status
        };

        if (membership != null)
        {
            organizationInfo.Role = (Role)membership.Role;
            organizationInfo.IsOwner = membership.IsOwner;
        }

        return organizationInfo;
    }
}