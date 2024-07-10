using Throw;

namespace DutyDock.Api.Client;

public class ApiClient : IDisposable
{
    protected ApiHttpClient? HttpClient { get; private set; }
    
    private bool _isDisposed;

    protected ApiClient(HttpClient httpClient)
    {
        httpClient.ThrowIfNull();

        HttpClient = ApiHttpClient.Create(httpClient);
    }
    
    protected ApiClient(ApiClientOptions options)
    {
        options.ThrowIfNull();

        HttpClient = ApiHttpClient.Create(options);
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        if (HttpClient != null)
        {
            try
            {
                HttpClient.Dispose();
            }
            catch (Exception)
            {
                // NOP: might happen for inflight request during client disposal
            }

            HttpClient = null;
        }

        _isDisposed = true;
    }
}