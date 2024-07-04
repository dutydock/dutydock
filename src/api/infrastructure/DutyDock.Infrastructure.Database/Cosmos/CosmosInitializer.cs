using Microsoft.Azure.Cosmos;
using Throw;
using CosmosDatabase = Microsoft.Azure.Cosmos.Database;

namespace DutyDock.Infrastructure.Database.Cosmos;

public static class CosmosInitializer
{
    private const int TtlEnabledNoExpiry = -1;
    private const int MinimumValueMaxRuAutoscale = 1000;

    private const string DefaultPartitionKeyPath = "/partitionKey";
    private const string IdPartitionKeyPath = "/id";

    public static async Task Execute(CosmosOptions options, CosmosClient client)
    {
        options.ThrowIfNull();
        client.ThrowIfNull();

        var mode = options.Mode;

        // Database
        var database = await EnsureDatabaseExists(options.Database, mode, options.DatabaseMaxRu, client);
        await EnsureDatabaseThroughput(database, mode, options.DatabaseMaxRu);

        // Data
        var dataContainer = await EnsureContainerExists(
            database, mode, options.DataContainer, DefaultPartitionKeyPath, TtlEnabledNoExpiry,
            options.DataContainerMaxRu);
        await EnsureContainerThroughput(dataContainer, mode, options.DataContainerMaxRu);

        var dataLeaseContainer = await EnsureContainerExists(
            database, mode, options.DataLeaseContainer, IdPartitionKeyPath, TtlEnabledNoExpiry,
            options.DataLeaseContainerMaxRu);
        await EnsureContainerThroughput(dataLeaseContainer, mode, options.DataLeaseContainerMaxRu);

        var dataErrorContainer = await EnsureContainerExists(
            database, mode, options.DataErrorContainer, IdPartitionKeyPath, TtlEnabledNoExpiry,
            options.DataErrorContainerMaxRu);
        await EnsureContainerThroughput(dataErrorContainer, mode, options.DataErrorContainerMaxRu);
    }

    private static async Task<CosmosDatabase> EnsureDatabaseExists(
        string? databaseName, CosmosOptions.ThroughputMode mode, int? maxRu, CosmosClient client)
    {
        databaseName.ThrowIfNull().IfEmpty();

        DatabaseResponse response;

        if (mode == CosmosOptions.ThroughputMode.Database)
        {
            maxRu.ThrowIfNull();
            maxRu.Value.Throw().IfLessThan(MinimumValueMaxRuAutoscale);

            // Set autoscale max RU/s
            var throughputProperties = ThroughputProperties.CreateAutoscaleThroughput(maxRu.Value);
            response = await client.CreateDatabaseIfNotExistsAsync(
                databaseName, throughputProperties: throughputProperties);
        }
        else
        {
            response = await client.CreateDatabaseIfNotExistsAsync(databaseName);
        }

        return response.Database;
    }

    private static async Task EnsureDatabaseThroughput(CosmosDatabase database, CosmosOptions.ThroughputMode mode,
        int? maxRu)
    {
        if (mode != CosmosOptions.ThroughputMode.Database)
        {
            return;
        }

        maxRu.ThrowIfNull();
        maxRu.Value.Throw().IfLessThan(MinimumValueMaxRuAutoscale);

        var currentThroughput = await database.ReadThroughputAsync();

        if (currentThroughput == null || currentThroughput != maxRu)
        {
            var throughputProperties = ThroughputProperties.CreateAutoscaleThroughput(maxRu.Value);
            await database.ReplaceThroughputAsync(throughputProperties);
        }
    }

    private static async Task<Container> EnsureContainerExists(
        CosmosDatabase database, CosmosOptions.ThroughputMode mode, string? containerName, string? partitionKeyPath,
        int? ttl = null,
        int? maxRu = null)
    {
        containerName.ThrowIfNull().IfEmpty();
        partitionKeyPath.ThrowIfNull().IfEmpty();

        var containerBuilder = database.DefineContainer(containerName, partitionKeyPath);

        if (ttl.HasValue)
        {
            containerBuilder.WithDefaultTimeToLive(ttl.Value);
        }

        // https://learn.microsoft.com/en-us/azure/cosmos-db/large-partition-keys
        containerBuilder.WithPartitionKeyDefinitionVersion(PartitionKeyDefinitionVersion.V2);

        ContainerResponse response;

        if (mode == CosmosOptions.ThroughputMode.Container)
        {
            maxRu.ThrowIfNull();
            maxRu.Value.Throw().IfLessThan(MinimumValueMaxRuAutoscale);

            // Set autoscale max RU/s
            var throughputProperties = ThroughputProperties.CreateAutoscaleThroughput(maxRu.Value);
            response = await containerBuilder.CreateIfNotExistsAsync(throughputProperties);
        }
        else
        {
            response = await containerBuilder.CreateIfNotExistsAsync();
        }

        return response.Container;
    }

    private static async Task EnsureContainerThroughput(Container container, CosmosOptions.ThroughputMode mode,
        int? maxRu = null)
    {
        if (mode != CosmosOptions.ThroughputMode.Container)
        {
            return;
        }

        maxRu.ThrowIfNull();
        maxRu.Value.Throw().IfLessThan(MinimumValueMaxRuAutoscale);

        var currentThroughput = await container.ReadThroughputAsync();

        if (currentThroughput == null || currentThroughput != maxRu)
        {
            var throughputProperties = ThroughputProperties.CreateAutoscaleThroughput(maxRu.Value);
            await container.ReplaceThroughputAsync(throughputProperties);
        }
    }
}