using Microsoft.Azure.Cosmos;

namespace DutyDock.Infrastructure.Database.Cosmos.Data;

public class ContainerProvider
{
    public Container DataContainer { get; }

    public ContainerProvider(ClientProvider clientProvider, CosmosOptions options)
    {
        DataContainer = clientProvider.Client.GetContainer(options.Database, options.DataContainer);
    }
}