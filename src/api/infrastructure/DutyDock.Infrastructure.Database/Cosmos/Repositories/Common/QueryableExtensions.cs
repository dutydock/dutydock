using DutyDock.Domain.Common.Models.Entities;
using DutyDock.Infrastructure.Database.Common;
using DutyDock.Infrastructure.Database.Common.Specifications;
using DutyDock.Infrastructure.Database.Cosmos.Data;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace DutyDock.Infrastructure.Database.Cosmos.Repositories.Common;

public static class QueryableExtensions
{
    public static async Task<QueryableResult<TEntity>> ExecuteAsync<TEntity>(
        this IQueryable<DataObject<TEntity>> query,
        CancellationToken cancellationToken = default)
        where TEntity : Entity
    {
        return await query.ExecuteAsync(null, false, cancellationToken);
    }

    public static async Task<QueryableResult<TEntity>> ExecuteAsync<TEntity>(
        this IQueryable<DataObject<TEntity>> query,
        Specification<TEntity>? specification = null,
        bool paged = false,
        CancellationToken cancellationToken = default)
        where TEntity : Entity
    {
        if (specification != null)
        {
            query = SpecificationEvaluator.GetQuery(query, specification);
        }

        string? continuationToken = null;
        using var iterator = query.ToFeedIterator();

        var entities = new List<DataObject<TEntity>>();

        try
        {
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync(cancellationToken);

                entities.AddRange(response);
                continuationToken = response.ContinuationToken;

                if (paged)
                {
                    break;
                }
            }
        }
        catch (CosmosException ex)
        {
            throw ex.Evaluate();
        }

        return new QueryableResult<TEntity>
        {
            Entities = entities,
            ContinuationToken = continuationToken
        };
    }
}