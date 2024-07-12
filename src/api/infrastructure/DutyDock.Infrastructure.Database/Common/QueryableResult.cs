using DutyDock.Domain.Common.Models.Entities;

namespace DutyDock.Infrastructure.Database.Common;

public class QueryableResult<TEntity> where TEntity : Entity
{
    public List<DataObject<TEntity>> Entities { get; set; } = new();

    public string? ContinuationToken { get; set; }
}