using DutyDock.Api.Client.Common;
using Throw;

namespace DutyDock.Api.Client.Handlers;

public class ApiEndpointRequestHandler : DelegatingHandler
{
    private readonly Func<IApiEndpointProvider?> _apiEndpointProviderFunc;
    
    public ApiEndpointRequestHandler(Func<IApiEndpointProvider?> apiEndpointProviderFunc)
    {
        _apiEndpointProviderFunc = apiEndpointProviderFunc;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var apiEndpointProvider = _apiEndpointProviderFunc();
        apiEndpointProvider.ThrowIfNull();

        var apiEndpoint = await apiEndpointProvider.GetApiEndpoint();

        if (apiEndpoint != null)
        {
            var baseUri = new Uri(apiEndpoint);
            var fullUri = new Uri(baseUri, request.RequestUri?.OriginalString);

            request.RequestUri = fullUri;
        }
        
        return await base.SendAsync(request, cancellationToken);
    }
}