using System.ComponentModel.DataAnnotations;

namespace DutyDock.Infrastructure;

public sealed record AppOptions
{
    public const string Section = "App";

    public sealed record DomainOptions
    {
        [Required] public string? Web { get; set; }

        [Required] public string? Api { get; set; }
    }

    [Required] public string? Name { get; set; }

    [Required] public string? Version { get; set; }

    [Required] public DomainOptions? Domain { get; set; }
}