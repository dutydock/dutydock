using System.Net;
using DutyDock.Domain.Common.Models.Entities;
using DutyDock.Domain.Common.Models.Events;
using DutyDock.Infrastructure.Database.Common;
using DutyDock.Infrastructure.Database.Common.Outbox;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Throw;

namespace DutyDock.Infrastructure.Database.Cosmos.Data;

public class DataContainerContext
{
    private const string EventTypeName = "event";

    private readonly ILogger<DataContainerContext> _logger;
    private readonly OutboxOptions _options;

    private readonly Container _dataContainer;

    public DataContainerContext(
        ILogger<DataContainerContext> logger,
        IOptions<OutboxOptions> options,
        ContainerProvider containerProvider)
    {
        _logger = logger;
        _options = options.Value;

        _dataContainer = containerProvider.DataContainer;
    }

    public List<IDataObject<Entity>> DataObjects { get; } = new();

    public void Add(IDataObject<Entity> entity)
    {
        if (DataObjects.FindIndex(0, match =>
                match.Id == entity.Id && match.PartitionKey == entity.PartitionKey) == -1)
        {
            DataObjects.Add(entity);
        }
    }

    public async Task<List<IDataObject<Entity>>> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        RaiseDomainEvents(DataObjects);

        return DataObjects.Count switch
        {
            1 => await SaveSingleAsync(DataObjects[0], cancellationToken),
            > 1 => await SaveInTransactionalBatchAsync(cancellationToken),
            _ => new List<IDataObject<Entity>>()
        };
    }

    private void RaiseDomainEvents(List<IDataObject<Entity>> dataObjects)
    {
        var eventDataObjects = new List<IDataObject<DomainEvent>>();

        foreach (var dataObject in dataObjects)
        {
            if (dataObject.Data is not IDomainEventEmitter<DomainEvent> eventEmitter)
            {
                continue;
            }

            eventDataObjects.AddRange(eventEmitter.Events.Select(domainEvent =>
                new DataObject<DomainEvent>(
                    domainEvent.Id,
                    dataObject.PartitionKey,
                    EventTypeName,
                    domainEvent,
                    null,
                    (int)_options.EventTtlInSec,
                    EntityState.Created)));
        }

        foreach (var eventDataObject in eventDataObjects)
        {
            Add(eventDataObject);
        }
    }

    private async Task<List<IDataObject<Entity>>> SaveInTransactionalBatchAsync(CancellationToken cancellationToken)
    {
        if (DataObjects.Count > 0)
        {
            // As per transaction requirements, all entities share the same partition key
            EnsureSinglePartitionKey();
            var partitionKey = new PartitionKey(DataObjects[0].PartitionKey);

            var batch = _dataContainer.CreateTransactionalBatch(partitionKey);

            DataObjects.ForEach(obj =>
            {
                TransactionalBatchItemRequestOptions? requestOptions = null;

                if (!string.IsNullOrWhiteSpace(obj.Etag))
                {
                    requestOptions = new TransactionalBatchItemRequestOptions { IfMatchEtag = obj.Etag };
                }

                switch (obj.State)
                {
                    case EntityState.Created:
                        batch.CreateItem(obj);
                        break;
                    case EntityState.Updated:
                        batch.ReplaceItem(obj.Id, obj, requestOptions);
                        break;
                    case EntityState.Unmodified:
                    case EntityState.Deleted:
                    default:
                        throw new NotImplementedException(obj.State.ToString());
                }
            });

            var batchResult = await batch.ExecuteAsync(cancellationToken);

            if (!batchResult.IsSuccessStatusCode)
            {
                _logger.LogWarning("Batch update failed for partition key {PartitionKey}: {Response}",
                    partitionKey, batchResult.Stringify());

                for (var i = 0; i < DataObjects.Count; i++)
                {
                    var objResult = batchResult[i];

                    _logger.LogWarning("Batch item {Item}", objResult.Stringify());

                    // 424 Failed dependency
                    // When a document operation fails within the transactional scope of a TransactionalBatch operation,
                    // all other operations within the batch are considered failed dependencies.
                    // This status code indicates that the current operation was considered failed because of another
                    // failure within the same transactional scope.
                    if (objResult.StatusCode == HttpStatusCode.FailedDependency)
                    {
                        continue;
                    }

                    // Not recoverable - clear context
                    DataObjects.Clear();
                    throw batchResult[i].StatusCode.Evaluate(etag: objResult.ETag);
                }
            }

            for (var i = 0; i < DataObjects.Count; i++)
            {
                DataObjects[i].Etag = batchResult[i].ETag;
            }
        }

        // Return copy of list as result
        var result = new List<IDataObject<Entity>>(DataObjects);

        // Work has been successfully done
        DataObjects.Clear();
        return result;
    }

    private async Task<List<IDataObject<Entity>>> SaveSingleAsync(IDataObject<Entity> dataObject,
        CancellationToken cancellationToken)
    {
        var requestOptions = new ItemRequestOptions
        {
            EnableContentResponseOnWrite = false
        };

        if (!string.IsNullOrWhiteSpace(dataObject.Etag))
        {
            requestOptions.IfMatchEtag = dataObject.Etag;
        }

        var partitionKey = new PartitionKey(dataObject.PartitionKey);

        try
        {
            ItemResponse<IDataObject<Entity>> response;

            switch (dataObject.State)
            {
                case EntityState.Created:
                    response = await _dataContainer.CreateItemAsync(dataObject, partitionKey, requestOptions,
                        cancellationToken);
                    break;
                case EntityState.Updated:
                    response = await _dataContainer.ReplaceItemAsync(dataObject, dataObject.Id, partitionKey,
                        requestOptions, cancellationToken);
                    break;
                case EntityState.Unmodified:
                case EntityState.Deleted:
                default:
                    DataObjects.Clear();
                    return new List<IDataObject<Entity>>();
            }

            dataObject.Etag = response.ETag;
            var result = new List<IDataObject<Entity>>(1) { dataObject };

            // Work has been successfully done
            DataObjects.Clear();
            return result;
        }
        catch (CosmosException ex)
        {
            DataObjects.Clear();
            throw ex.Evaluate(dataObject.Id, dataObject.Etag);
        }
    }

    private void EnsureSinglePartitionKey()
    {
        var keys = DataObjects.Select(obj => obj.PartitionKey).Distinct().ToList();
        var count = keys.Count;

        if (count == 1)
        {
            return;
        }

        _logger.LogError("Different partition keys found in transactional batch");

        foreach (var key in keys)
        {
            _logger.LogError("Partition key {PartitionKey}", key);
        }

        count.Throw();
    }
}