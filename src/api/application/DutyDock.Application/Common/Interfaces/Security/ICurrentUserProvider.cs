namespace DutyDock.Application.Common.Interfaces.Security;

public interface ICurrentUserProvider
{
    string UserId { get; }

    string UserSecurityStamp { get; }

    string OrganizationId { get; }

    string? TryGetOrganizationId { get; }
    
    string MembershipSecurityStamp { get; }
}