using DutyDock.Api.Client.Common;
using DutyDock.Api.Client.Handlers;
using Throw;

namespace DutyDock.Api.Client;

public abstract class ApiClientBuilder<T> where T : ApiClient
{
    protected ApiClientOptions ClientOptions = ApiClientOptions.Default();

    public ApiClientBuilder<T> WithApiEndpoint(IApiEndpointProvider provider)
    {
        ClientOptions = ClientOptions with { ApiEndpointProvider = provider };
        return this;
    }

    public ApiClientBuilder<T> WithRequestTimeout(TimeSpan requestTimeout)
    {
        ClientOptions = ClientOptions with { RequestTimeout = requestTimeout };
        return this;
    }

    public ApiClientBuilder<T> WithCookieHandling()
    {
        ClientOptions = ClientOptions with { UseCookieHandler = true };
        return this;
    }

    public ApiClientBuilder<T> WithApiKey(string apiKey)
    {
        ClientOptions = ClientOptions with { ApiKey = apiKey };
        return this;
    }

    public ApiClientBuilder<T> WithResponseHandler(Func<IResponseHandler?> provider)
    {
        provider.ThrowIfNull();
        
        ClientOptions = ClientOptions with { ResponseHandlerProvider = provider };
        return this;
    }

    public ApiClientBuilder<T> WithBearerToken(Func<IBearerTokenProvider> provider)
    {
        provider.ThrowIfNull();
        
        ClientOptions = ClientOptions with { BearerTokenProvider = provider };
        return this;
    }

    public abstract T Build();
}