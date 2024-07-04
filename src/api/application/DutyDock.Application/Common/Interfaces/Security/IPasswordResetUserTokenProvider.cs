using ErrorOr;

namespace DutyDock.Application.Common.Interfaces.Security;

public interface IPasswordResetUserTokenProvider
{
    string Generate(string userId, string securityStamp);

    ErrorOr<Success> Validate(string token, string userId, string securityStamp);
}