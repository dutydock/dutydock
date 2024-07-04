using Azure.Identity;
using DutyDock.Infrastructure.Shared;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Throw;

namespace DutyDock.Infrastructure.Security.DataProtection;

public static class Setup
{
    public static IServiceCollection AddDataProtectionConfig(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.ThrowIfNull();
        configuration.ThrowIfNull();

        const string sectionName = $"{SecurityOptions.Section}:{DataProtectionOptions.Section}";
        services.ConfigureAndValidate<DataProtectionOptions>(sectionName, configuration);

        var options = configuration.GetSection(sectionName).Get<DataProtectionOptions>();
        options.ThrowIfNull();

        switch (options.Mode)
        {
            case DataProtectionOptions.DataProtectionMode.Default:
                AddDefaultDataProtection(services);
                break;
            case DataProtectionOptions.DataProtectionMode.AzureBlobWithKeyVault:
                AddAzureBlobWithKeyVaultDataProtection(services, options);
                break;
            default:
                throw new NotImplementedException($"Data protection mode '{options.Mode}' is not supported.");
        }

        return services;
    }

    private static void AddDefaultDataProtection(IServiceCollection services)
    {
        services.AddDataProtection();
    }

    private static void AddAzureBlobWithKeyVaultDataProtection(IServiceCollection services,
        DataProtectionOptions options)
    {
        var protectionOptions = options.BlobWithKeyVault;
        protectionOptions.ThrowIfNull();

        var connectionString = protectionOptions.ConnectionString;
        var containerName = protectionOptions.ContainerName;
        var blobName = protectionOptions.BlobName;

        var keyName = protectionOptions.KeyName;
        var vaultName = protectionOptions.VaultName;

        var keyIdentifier = $"https://{vaultName}.vault.azure.net/keys/{keyName}/";

        services.AddDataProtection()
            .PersistKeysToAzureBlobStorage(connectionString, containerName, blobName)
            .ProtectKeysWithAzureKeyVault(new Uri(keyIdentifier), new DefaultAzureCredential());
    }
}