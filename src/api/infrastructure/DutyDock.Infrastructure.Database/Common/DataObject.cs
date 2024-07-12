using DutyDock.Domain.Common.Models.Entities;
using DutyDock.Infrastructure.Database.Cosmos.Data;
using Newtonsoft.Json;

namespace DutyDock.Infrastructure.Database.Common;

public class DataObject<TEntity> : IDataObject<TEntity> where TEntity : Entity
{
    public DataObject(
        string id,
        string partitionKey,
        string objectType,
        TEntity data,
        string? eTag = Constants.NoEtag,
        int ttl = Constants.InfiniteTtl,
        EntityState state = EntityState.Unmodified)
    {
        Id = id;
        PartitionKey = partitionKey;
        Type = objectType;
        Data = data;
        Etag = eTag;
        Ttl = ttl;
        State = state;
    }

    [JsonProperty("id")] public string Id { get; private set; }

    [JsonProperty("partitionKey")] public string PartitionKey { get; private set; }

    [JsonProperty("type")] public string Type { get; private set; }

    [JsonProperty("data")] public TEntity Data { get; set; }

    [JsonProperty("_etag")] public string? Etag { get; set; }

    // Time to live value is configured in seconds.
    [JsonProperty("ttl")] public int Ttl { get; private set; }

    [JsonIgnore] public EntityState State { get; set; }
}