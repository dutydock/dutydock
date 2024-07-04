using DutyDock.Application.Common.Interfaces.Security;
using DutyDock.Application.Common.Interfaces.Services;
using ErrorOr;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

namespace DutyDock.Infrastructure.Security.Tokens;

/**
 * See reference implementation at:
 * https://github.com/aspnet/Identity/blob/release/2.2/src/Identity/DataProtectionTokenProvider.cs
 */
public sealed class PasswordResetUserTokenProvider : UserTokenProvider, IPasswordResetUserTokenProvider
{
    private const string PasswordResetPurpose = "password-reset";

    public PasswordResetUserTokenProvider(
        IOptions<TokenOptions> options,
        IDataProtectionProvider dataProtectionProvider,
        IDateTimeProvider dateTimeProvider) : base(options, dataProtectionProvider, dateTimeProvider)
    {
    }

    protected override string Purpose => PasswordResetPurpose;

    public string Generate(string userId, string securityStamp)
    {
        return base.Generate(userId, securityStamp);
    }

    public ErrorOr<Success> Validate(string token, string userId, string securityStamp)
    {
        BinaryReader? reader = null;

        try
        {
            reader = GetTokenReader(token);

            var expirationResult = ValidateExpiration(reader);

            if (expirationResult.IsError)
            {
                return expirationResult;
            }

            var userResult = ValidateUser(reader, userId);

            if (userResult.IsError)
            {
                return expirationResult;
            }

            var purposeResult = ValidatePurpose(reader);

            if (purposeResult.IsError)
            {
                return purposeResult;
            }

            var securityStampResult = ValidateSecurityStamp(reader, securityStamp);

            if (securityStampResult.IsError)
            {
                return securityStampResult;
            }

            return ValidatePadding(reader);
        }
        catch
        {
            return Error.Unexpected();
        }
        finally
        {
            reader?.Close();
        }
    }

    private static ErrorOr<Success> ValidateSecurityStamp(BinaryReader reader, string securityStamp)
    {
        var tokenSecurityStamp = reader.ReadString();

        if (tokenSecurityStamp != securityStamp)
        {
            return TokenErrors.InvalidSecurityStamp;
        }

        return Result.Success;
    }
}