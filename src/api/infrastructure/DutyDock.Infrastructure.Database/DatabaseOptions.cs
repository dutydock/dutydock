using System.ComponentModel.DataAnnotations;
using DutyDock.Infrastructure.Database.Common.Outbox;
using DutyDock.Infrastructure.Database.Cosmos;

namespace DutyDock.Infrastructure.Database;

public sealed record DatabaseOptions
{
    public const string Section = "Database";

    public enum DatabaseType
    {
        Cosmos
    }

    [Required] public DatabaseType Type { get; set; }

    public CosmosOptions? Cosmos { get; set; }

    [Required] public OutboxOptions? Outbox { get; set; }
}