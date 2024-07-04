using ErrorOr;

namespace DutyDock.Infrastructure.Security.Tokens;

public static class TokenErrors
{
    public static readonly Error Expired =
        Error.Validation("Security.UserTokenExpired", "User token expired");

    public static readonly Error InvalidUser =
        Error.Validation("Security.UserTokenInvalidUser", "User token invalid user");

    public static readonly Error InvalidPurpose =
        Error.Validation("Security.UserTokenInvalidPurpose", "User token invalid purpose");

    public static readonly Error InvalidSecurityStamp =
        Error.Validation("Security.UserTokenInvalidSecurityStamp", "User token invalid security stamp");

    public static readonly Error InvalidOrganization =
        Error.Validation("Security.UserTokenInvalidOrganization", "User token invalid organization");

    public static readonly Error Padded =
        Error.Validation("Security.UserTokenPadded", "User token was padded");
}