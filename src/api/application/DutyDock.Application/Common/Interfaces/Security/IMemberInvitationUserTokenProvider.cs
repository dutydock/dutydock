using ErrorOr;

namespace DutyDock.Application.Common.Interfaces.Security;

public interface IMemberInvitationUserTokenProvider
{
    string Generate(string userId, string organizationId);
    
    ErrorOr<Success> Validate(string token, string userId, string organizationId);
}