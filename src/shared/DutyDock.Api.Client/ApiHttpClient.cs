using System.Net.Mime;
using DutyDock.Api.Client.Handlers;
using Microsoft.Net.Http.Headers;
using Throw;
using CacheControlHeaderValue = System.Net.Http.Headers.CacheControlHeaderValue;

namespace DutyDock.Api.Client;

public sealed class ApiHttpClient : IDisposable
{
    private readonly HttpClient _httpClient;

    private bool _disposedValue;

    private ApiHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public static ApiHttpClient Create(HttpClient httpClient)
    {
        return new ApiHttpClient(httpClient);
    }
    
    public static ApiHttpClient Create(ApiClientOptions options)
    {
        options.ApiEndpointProvider.ThrowIfNull();
        
        var pipeline = (HttpMessageHandler)new HttpClientHandler();
        
        if (options.UseCookieHandler)
        {
            pipeline = pipeline.DecorateWith(new CookieHandler());
        }

        if (options.BearerTokenProvider != null)
        {
            pipeline = pipeline.DecorateWith(new BearerTokenRequestHandler(options.BearerTokenProvider));
        }

        if (options.ResponseHandlerProvider != null)
        {
            pipeline = pipeline.DecorateWith(new UserResponseHandler(options.ResponseHandlerProvider));
        }

        var httpClient = new HttpClient(pipeline)
        {
            Timeout = options.RequestTimeout ?? TimeSpan.FromSeconds(5)
        };

        httpClient.DefaultRequestHeaders.CacheControl =
            new CacheControlHeaderValue { NoCache = true };

        httpClient.DefaultRequestHeaders.Add(
            HeaderNames.Accept, MediaTypeNames.Application.Json);
        
        if (!string.IsNullOrEmpty(options.ApiKey))
        {
            httpClient.DefaultRequestHeaders.Add("X-Api-Key", options.ApiKey);
        }
        
        return new ApiHttpClient(httpClient);
    }

    public async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        return await _httpClient.SendAsync(request, cancellationToken);
    }

    private void ThrowIfDisposed()
    {
        if (_disposedValue)
        {
            throw new ObjectDisposedException(nameof(ApiHttpClient));
        }
    }

    public void Dispose()
    {
        Dispose(true);
    }

    private void Dispose(bool disposing)
    {
        if (_disposedValue)
        {
            return;
        }

        if (disposing)
        {
            _httpClient.Dispose();
        }

        _disposedValue = true;
    }
}