using System.Net.Http.Headers;
using DutyDock.Api.Client.Common;
using Throw;

namespace DutyDock.Api.Client.Handlers;

internal class BearerTokenRequestHandler : DelegatingHandler
{
    private readonly Func<IBearerTokenProvider> _tokenProviderFunc;

    public BearerTokenRequestHandler(Func<IBearerTokenProvider> tokenProviderFunc)
    {
        _tokenProviderFunc = tokenProviderFunc;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var tokenProvider = _tokenProviderFunc();
        tokenProvider.ThrowIfNull();

        var bearerToken = await tokenProvider.GetToken();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

        return await base.SendAsync(request, cancellationToken);
    }
}