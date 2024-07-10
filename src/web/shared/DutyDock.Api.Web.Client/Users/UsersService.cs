using DutyDock.Api.Client;
using DutyDock.Api.Client.Common;
using DutyDock.Api.Client.Services;
using DutyDock.Api.Contracts.Dto.Users;
using DutyDock.Api.Web.Contracts.Users;

namespace DutyDock.Api.Web.Client.Users;

public class UsersService : BaseService, IUsersService
{
    private const string BasePath = "api/users";

    public UsersService(IApiEndpointProvider? endpointProvider, ApiHttpClient? httpClient)
        : base(endpointProvider, httpClient, BasePath)
    {
    }

    public async Task<ApiDataResult<AuthenticationDetails>> SignIn(SignInRequest request,
        CancellationToken cancellationToken = default)
    {
        const string url = $"{BasePath}/sign-in";
        return await SendForDataAsync<AuthenticationDetails>(HttpMethod.Post, url, request, cancellationToken);
    }

    public async Task<ApiResult> SignOut(CancellationToken cancellationToken = default)
    {
        const string url = $"{BasePath}/sign-out";
        return await SendAsync(HttpMethod.Post, url, cancellationToken: cancellationToken);
    }
}