using DutyDock.Application.Common.Errors;
using ErrorOr;

namespace DutyDock.Application.Common.Authorization;

public static class AuthorizationErrors
{
    public static readonly Error NotValidated =
        CustomError.Forbidden("Authorization.NotValidated", "Email address is not validated");
    
    public static readonly Error UserSecurityStamp =
        CustomError.Forbidden("Authorization.UserSecurityStamp", "User security stamp has changed");

    public static readonly Error MembershipSecurityStamp =
        CustomError.Forbidden("Authorization.MembershipSecurityStamp", "Membership security stamp has changed");

    public static readonly Error NotActiveMember =
        CustomError.Forbidden("Authorization.NotActiveMember", "User is not an active member of the organization");

    public static readonly Error MembershipRole =
        CustomError.Forbidden("Authorization.MembershipRole", "User membership role is not permitted");
    
    public static readonly Error NotOwner =
        CustomError.Forbidden("Authorization.NotOwner", "User is not the owner of the organization");
}