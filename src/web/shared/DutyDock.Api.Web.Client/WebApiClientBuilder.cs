using DutyDock.Api.Client;

namespace DutyDock.Api.Web.Client;

public sealed class WebApiClientBuilder : ApiClientBuilder<WebApiClient>
{
    public override WebApiClient Build()
    {
        return new WebApiClient(ClientOptions);
    }
}