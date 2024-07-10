using DutyDock.Api.Contracts.Dto.Organizations;

namespace DutyDock.Api.Contracts.Dto.Users;

public sealed record OrganizationInfo
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public OrganizationStatus Status { get; set; }
    
    public Role Role { get; set; }

    public bool IsOwner { get; set; }
}