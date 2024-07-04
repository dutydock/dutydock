namespace DutyDock.Application.Common.Database.Common;

public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken cancellationToken = default);
}