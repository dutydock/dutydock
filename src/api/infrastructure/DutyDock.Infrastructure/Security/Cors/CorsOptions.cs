namespace DutyDock.Infrastructure.Security.Cors;

public sealed record CorsOptions
{
    public const string Section = "Cors";

    public List<string>? AllowedOrigins { get; set; }
}