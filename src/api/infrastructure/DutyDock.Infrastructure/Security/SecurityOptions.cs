using DutyDock.Infrastructure.Security.Cors;
using Microsoft.AspNetCore.DataProtection;

namespace DutyDock.Infrastructure.Security;

public sealed record SecurityOptions
{
    public const string Section = "Security";
    
    public CorsOptions? Cors { get; set; }

    public DataProtectionOptions? DataProtection { get; set; }
}