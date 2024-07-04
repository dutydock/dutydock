using System.ComponentModel.DataAnnotations;

namespace DutyDock.Infrastructure.Database.Cosmos;

public sealed record CosmosOptions
{
    public const string Section = "Cosmos";
    
    public enum ThroughputMode
    {
        Serverless,
        Database,
        Container
    }

    [Required] public string? Endpoint { get; set; }
    
    [Required] public string? AccessKey { get; set; }
    
    [Required] public bool IsEmulator { get; set; }

    [Required] public ThroughputMode Mode { get; set; }
    
    [Required] public string? Database { get; set; }
    
    public int? DatabaseMaxRu { get; set; }
    
    [Required] public string? DataContainer { get; set; }

    public int? DataContainerMaxRu { get; set; }

    [Required] public string? DataLeaseContainer { get; set; }
    
    public int? DataLeaseContainerMaxRu { get; set; }
    
    [Required] public string? DataErrorContainer { get; set; }

    public int? DataErrorContainerMaxRu { get; set; }
    
}