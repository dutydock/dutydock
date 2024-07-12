using DutyDock.Application.Common.Database.Common;

namespace DutyDock.Infrastructure.Database.Cosmos.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly DataContainerContext _context;

    public UnitOfWork(DataContainerContext context)
    {
        _context = context;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}