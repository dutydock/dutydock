using DutyDock.Api.Client.Common;
using DutyDock.Api.Contracts.Dto.Users;
using DutyDock.Api.Web.Contracts.Users;

namespace DutyDock.Api.Web.Client.Users;

public interface IUsersService
{
    Task<ApiDataResult<AuthenticationDetails>> SignIn(SignInRequest request,
        CancellationToken cancellationToken = default);
    
    Task<ApiResult> SignOut(CancellationToken cancellationToken = default);
}