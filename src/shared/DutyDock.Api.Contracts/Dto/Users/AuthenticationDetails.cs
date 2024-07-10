namespace DutyDock.Api.Contracts.Dto.Users;

public sealed record AuthenticationDetails
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public bool IsValidated { get; set; }

    public required string Culture { get; set; }

    public required string TimeZone { get; set; }

    public required string Language { get; set; }

    public OrganizationInfo? Organization { get; set; }
}