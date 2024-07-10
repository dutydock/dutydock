using DutyDock.Api.Contracts.Dto.Users;

namespace DutyDock.Web.Core.Models.Security;

public sealed record UserProfile
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public bool IsValidated { get; init; }

    public required string Culture { get; init; }

    public required string TimeZone { get; init; }

    public required string Language { get; init; }

    public override string ToString()
    {
        return $"{Name} ({Id})";
    }

    public static UserProfile Map(AuthenticationDetails authDetails)
    {
        return new UserProfile
        {
            Id = authDetails.Id,
            Name = authDetails.Name,
            IsValidated = authDetails.IsValidated,
            Culture = authDetails.Culture,
            TimeZone = authDetails.TimeZone,
            Language = authDetails.Language
        };
    }
}