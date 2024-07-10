using DutyDock.Api.Client;
using DutyDock.Api.Client.Common;
using DutyDock.Api.Web.Client.Users;

namespace DutyDock.Api.Web.Client;

public class WebApiClient : ApiClient, IWebApiClient
{
    public IUsersService Users { get; private set; } = null!;
    
    public WebApiClient(HttpClient httpClient) : base(httpClient)
    {
        CreateServices();
    }
    
    public WebApiClient(ApiClientOptions options) : base(options)
    {
        CreateServices(options.ApiEndpointProvider);
    }
    
    private void CreateServices(IApiEndpointProvider? endpointProvider = null)
    { 
        Users = new UsersService(endpointProvider, HttpClient);
    }
}
