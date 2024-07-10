using DutyDock.Api.Client.Common;
using DutyDock.Api.Client.Handlers;

namespace DutyDock.Api.Client;

public sealed record ApiClientOptions(
    IApiEndpointProvider ApiEndpointProvider,
    TimeSpan? RequestTimeout,
    bool UseCookieHandler,
    string? ApiKey,
    Func<IResponseHandler?>? ResponseHandlerProvider,
    Func<IBearerTokenProvider>? BearerTokenProvider)
{
    public static ApiClientOptions Default()
    {
        return new ApiClientOptions(
            new ApiEndpointProvider(null), 
            null, 
            false, 
            null, 
            null, 
            null);
    }
}
