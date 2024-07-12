using DutyDock.Domain.Common.Models.Entities;

namespace DutyDock.Infrastructure.Database.Common.Specifications;

public static class SpecificationEvaluator
{
    public static IQueryable<DataObject<TEntity>> GetQuery<TEntity>(IQueryable<DataObject<TEntity>> inputQueryable,
        Specification<TEntity> specification) where TEntity : Entity
    {
        var queryable = inputQueryable;

        if (specification.Criteria is not null)
        {
            queryable = queryable.Where(specification.Criteria);
        }

        return queryable;
    }
}