using System.ComponentModel.DataAnnotations;

namespace DutyDock.Infrastructure.Database.Common.Outbox;

public sealed record OutboxOptions
{
    public const string Section = "Outbox";
    
    public enum QueueType
    {
        AzureServiceBus,
        InMemory
    }
    
    public sealed record AzureServiceBusOptions
    {
        [Required] public string? Name { get; set; }

        [Required] public string? SendConnectionString { get; set; }

        [Required] public string? ListenConnectionString { get; set; }
    }
    
    [Required] public QueueType EventQueueType { get; set; }
    
    // Between 10 seconds and 10 days
    [Required] [Range(10, 864000)] public uint EventTtlInSec { get; set; }
    
    public AzureServiceBusOptions? AzureServiceBusQueue { get; set; }
}