using System.ComponentModel.DataAnnotations;

namespace DutyDock.Infrastructure.Security.Authentication.Cookie;

public sealed record CookieOptions
{
    public const string Section = "Cookie";

    public string? Name { get; set; }

    public bool UseStrict { get; set; }

    [Required] public string? Domain { get; set; }

    [Range(1, 14)] public uint ExpiryInDays { get; set; }
}