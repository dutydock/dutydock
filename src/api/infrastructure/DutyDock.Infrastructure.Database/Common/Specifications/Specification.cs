using System.Linq.Expressions;
using DutyDock.Domain.Common.Models.Entities;

namespace DutyDock.Infrastructure.Database.Common.Specifications;

public abstract class Specification<TEntity> where TEntity : Entity
{
    protected Specification(Expression<Func<DataObject<TEntity>, bool>>? criteria = null)
    {
        Criteria = criteria;
    }

    public Expression<Func<DataObject<TEntity>, bool>>? Criteria { get; }
}