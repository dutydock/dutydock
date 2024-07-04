using System.ComponentModel.DataAnnotations;

namespace DutyDock.Infrastructure.Security.Tokens;

public sealed record TokenOptions
{
    public const string Section = "Tokens";
    
    [Range(1, 60)] public uint ExpiryInMinutes { get; set; }
}