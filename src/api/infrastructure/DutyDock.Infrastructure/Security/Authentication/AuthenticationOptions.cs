using DutyDock.Infrastructure.Security.Authentication.Cookie;

namespace DutyDock.Infrastructure.Security.Authentication;

public sealed record AuthenticationOptions
{
    public const string Section = "Authentication";

    public CookieOptions? Cookie { get; set; }
}