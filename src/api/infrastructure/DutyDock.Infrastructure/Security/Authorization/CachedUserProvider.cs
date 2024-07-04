using DutyDock.Application.Common.Database;
using DutyDock.Application.Common.Interfaces.Security;
using DutyDock.Domain.Iam.User;

namespace DutyDock.Infrastructure.Security.Authorization;

public class CachedUserProvider : ICachedUserProvider
{
    private readonly IUserRepository _userRepository;

    private User? _user;

    public CachedUserProvider(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> GetById(string? id, CancellationToken cancellationToken = default)
    {
        if (id == null)
        {
            return null;
        }

        if (_user != null)
        {
            return _user;
        }

        var (user, _) = await _userRepository.GetById(id, cancellationToken);

        _user = user;

        return _user;
    }
}