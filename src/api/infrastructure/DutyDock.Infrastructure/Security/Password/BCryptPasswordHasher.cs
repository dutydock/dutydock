using BCrypt.Net;
using DutyDock.Domain.Identity.User.Services;
using Throw;

namespace DutyDock.Infrastructure.Security.Password;

/// <summary>
/// Implementation of <see cref="IUserPasswordHasher"/> using bcrypt.
/// </summary>
/// <remarks>
/// See https://github.com/BcryptNet/bcrypt.net
/// We explicitly specify the hash type to protect against a breaking change on library upgrade.
/// </remarks>
public class BCryptPasswordHasher : IUserPasswordHasher
{
    public string Hash(string text)
    {
        text.ThrowIfNull();
        
        return BCrypt.Net.BCrypt.EnhancedHashPassword(text, HashType.SHA384);
    }

    public bool Verify(string text, string hash)
    {
        text.ThrowIfNull();
        hash.ThrowIfNull();
        
        // ReSharper disable once RedundantArgumentDefaultValue
        return BCrypt.Net.BCrypt.EnhancedVerify(text, hash, HashType.SHA384);
    }
}