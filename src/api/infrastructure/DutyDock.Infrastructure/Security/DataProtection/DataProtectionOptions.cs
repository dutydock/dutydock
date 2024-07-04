using System.ComponentModel.DataAnnotations;

namespace DutyDock.Infrastructure.Security.DataProtection;

public sealed record DataProtectionOptions
{
    public const string Section = "DataProtection";
    
    public enum DataProtectionMode
    {
        Default,
        AzureBlobWithKeyVault
    }
    
    public class AzureBlobWithKeyVaultOptions
    {
        [Required] public string? ConnectionString { get; set; }
        
        [Required] public string? ContainerName { get; set; }

        [Required] public string? BlobName { get; set; }

        [Required] public string? VaultName { get; set; }
        
        [Required] public string? KeyName { get; set; }
    }
    
    [Required] public DataProtectionMode Mode { get; set; }

    public AzureBlobWithKeyVaultOptions? BlobWithKeyVault { get; set; }
}