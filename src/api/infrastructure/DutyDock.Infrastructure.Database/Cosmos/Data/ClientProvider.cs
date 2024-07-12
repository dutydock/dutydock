using Microsoft.Azure.Cosmos;

namespace DutyDock.Infrastructure.Database.Cosmos.Data;

public class ClientProvider
{
    public CosmosClient Client { get; }
    
    public ClientProvider(CosmosClient client)
    {
        Client = client;
    }
}