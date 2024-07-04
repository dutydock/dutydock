using System.Text;
using DutyDock.Application.Common.Interfaces.Services;
using ErrorOr;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Throw;

namespace DutyDock.Infrastructure.Security.Tokens;

public abstract class UserTokenProvider
{
    private const string DataProtectorPurpose = "user-token-provider";

    private static readonly Encoding DefaultEncoding = new UTF8Encoding(false, true);

    private readonly TokenOptions _options;
    private readonly IDataProtector _dataProtector;
    private readonly IDateTimeProvider _dateTimeProvider;

    protected UserTokenProvider(IOptions<TokenOptions> options,
        IDataProtectionProvider dataProtectionProvider,
        IDateTimeProvider dateTimeProvider)
    {
        options.ThrowIfNull();
        dataProtectionProvider.ThrowIfNull();
        dateTimeProvider.ThrowIfNull();

        _options = options.Value;
        _dataProtector = dataProtectionProvider.CreateProtector(DataProtectorPurpose);
        _dateTimeProvider = dateTimeProvider;
    }

    protected abstract string Purpose { get; }

    protected string Generate(string userId, params string[] values)
    {
        var ms = new MemoryStream();

        using (var writer = new BinaryWriter(ms, DefaultEncoding, true))
        {
            writer.Write(_dateTimeProvider.UtcNow.Ticks);
            writer.Write(userId);
            writer.Write(Purpose);

            foreach (var value in values)
            {
                writer.Write(value);
            }
        }

        var protectedBytes = _dataProtector.Protect(ms.ToArray());
        return Convert.ToBase64String(protectedBytes);
    }

    protected BinaryReader GetTokenReader(string token)
    {
        var unprotectedData = _dataProtector.Unprotect(Convert.FromBase64String(token));
        var ms = new MemoryStream(unprotectedData);

        var reader = new BinaryReader(ms, DefaultEncoding, true);

        return reader;
    }

    protected ErrorOr<Success> ValidateExpiration(BinaryReader reader)
    {
        var creationTime = new DateTime(reader.ReadInt64());
        var expirationTime = creationTime + TimeSpan.FromMinutes(_options.ExpiryInMinutes);

        if (expirationTime < _dateTimeProvider.UtcNow)
        {
            return TokenErrors.Expired;
        }

        return Result.Success;
    }

    protected static ErrorOr<Success> ValidateUser(BinaryReader reader, string userId)
    {
        var tokenUserId = reader.ReadString();

        if (tokenUserId != userId)
        {
            return TokenErrors.InvalidUser;
        }

        return Result.Success;
    }

    protected ErrorOr<Success> ValidatePurpose(BinaryReader reader)
    {
        var tokenPurpose = reader.ReadString();

        if (tokenPurpose != Purpose)
        {
            return TokenErrors.InvalidPurpose;
        }

        return Result.Success;
    }

    protected static ErrorOr<Success> ValidatePadding(BinaryReader reader)
    {
        if (reader.PeekChar() != -1)
        {
            return TokenErrors.Padded;
        }

        return Result.Success;
    }
}