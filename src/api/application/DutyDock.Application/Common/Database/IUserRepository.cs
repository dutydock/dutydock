using DutyDock.Application.Common.Database.Common;
using DutyDock.Domain.Identity.User;

namespace DutyDock.Application.Common.Database;

public interface IUserRepository : IRepository<User>
{
    Task<(User?, string?)> GetByEmailAddress(string emailAddress, CancellationToken cancellationToken = default);
}