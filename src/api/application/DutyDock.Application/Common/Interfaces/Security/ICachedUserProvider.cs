using DutyDock.Domain.Identity.User;

namespace DutyDock.Application.Common.Interfaces.Security;

public interface ICachedUserProvider
{
    Task<User?> GetById(string? id, CancellationToken cancellationToken = default);
}