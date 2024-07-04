using DutyDock.Domain.Common.Models.Entities;

namespace DutyDock.Application.Common.Database.Common;

public interface IRepository<TEntity> where TEntity : Entity
{
    Task<(TEntity?, string?)> GetById(
        string id,
        CancellationToken cancellationToken = default);
}