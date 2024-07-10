using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace DutyDock.Api.Client.Handlers;

/**
 * This handler includes the 'withCredential' property on Ajax/XMLHttpRequest.
 * The property is needed when performing cross-origin requests, to ensure
 * cookies are sent to the backend.
 *
 * Correspondingly, the backend cors policy must include 'AllowCredentials' in such case.
 *
 * Reference: https://developer.mozilla.org/en-US/docs/Web/API/XMLHttpRequest/withCredentials
 */
public class CookieHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        return base.SendAsync(request, cancellationToken);
    }
}