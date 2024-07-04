using DutyDock.Application.Common.Interfaces.Security;
using DutyDock.Application.Common.Interfaces.Services;
using ErrorOr;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

namespace DutyDock.Infrastructure.Security.Tokens;

public class MemberInvitationUserTokenProvider : UserTokenProvider, IMemberInvitationUserTokenProvider
{
    private const string MemberInvitationPurpose = "member-invitation";

    public MemberInvitationUserTokenProvider(
        IOptions<TokenOptions> options,
        IDataProtectionProvider dataProtectionProvider,
        IDateTimeProvider dateTimeProvider) : base(options, dataProtectionProvider, dateTimeProvider)
    {
    }

    protected override string Purpose => MemberInvitationPurpose;

    public string Generate(string userId, string organizationId)
    {
        return base.Generate(userId, organizationId);
    }

    public ErrorOr<Success> Validate(string token, string userId, string organizationId)
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

            var organizationResult = ValidateOrganization(reader, organizationId);

            if (organizationResult.IsError)
            {
                return organizationResult;
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

    private static ErrorOr<Success> ValidateOrganization(BinaryReader reader, string organizationId)
    {
        var tokenOrganizationId = reader.ReadString();

        if (tokenOrganizationId != organizationId)
        {
            return TokenErrors.InvalidOrganization;
        }

        return Result.Success;
    }
}