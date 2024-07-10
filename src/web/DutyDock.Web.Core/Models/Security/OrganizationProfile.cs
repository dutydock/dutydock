using DutyDock.Api.Contracts.Dto.Organizations;
using DutyDock.Api.Contracts.Dto.Users;

namespace DutyDock.Web.Core.Models.Security;

public sealed record OrganizationProfile
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public OrganizationStatus Status { get; init; }

    public Role Role { get; init; }

    public bool IsOwner { get; init; }

    public static OrganizationProfile? Map(OrganizationInfo? info)
    {
        if (info == null)
        {
            return null;
        }

        return new OrganizationProfile
        {
            Id = info.Id,
            Name = info.Name,
            Status = info.Status,
            Role = info.Role,
            IsOwner = info.IsOwner
        };
    }
}