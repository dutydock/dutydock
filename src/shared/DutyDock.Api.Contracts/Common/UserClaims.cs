namespace DutyDock.Api.Contracts.Common;

public static class UserClaims
{
    public const string Name = "Name";
    public const string IsValidated = "IsValidated";
    
    public const string UserId = "UserId";
    public const string UserSecurityStamp = "UserSecurityStamp";

    public const string OrganizationId = "OrganizationId";
    public const string MembershipSecurityStamp = "MembershipSecurityStamp";
    
    public const string Role = "Role";
    public const string IsOwner = "IsOwner";
}