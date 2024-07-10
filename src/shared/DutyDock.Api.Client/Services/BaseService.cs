using System.Net;
using System.Net.Mime;
using System.Text;
using DutyDock.Api.Client.Common;
using DutyDock.Api.Contracts.Common;

namespace DutyDock.Api.Client.Services;

public abstract class BaseService
{
    private readonly IApiEndpointProvider? _endpointProvider;
    private readonly ApiHttpClient? _httpClient;
    private readonly string _basePath;
    
    protected BaseService(
        IApiEndpointProvider? endpointProvider, 
        ApiHttpClient? httpClient, 
        string basePath)
    {
        _endpointProvider = endpointProvider;
        _httpClient = httpClient;
        _basePath = basePath;
    }

    protected string GetBasePath()
    {
        return _basePath;
    }

    protected string GetResolvedBasePath(params string[] ids)
    {
        var objectIds = ids.Select(s => (object)s).ToArray();
        return string.Format($"{_basePath}", objectIds);
    }

    protected async Task<ApiResult> SendAsync(
        HttpMethod method,
        string uri,
        object? payload = default,
        CancellationToken cancellationToken = default)
    {
        return await SendAsync(method, uri, payload, null, cancellationToken);
    }

    protected async Task<ApiDataResult<T>> SendForDataAsync<T>(
        HttpMethod method,
        string uri,
        object? payload = default,
        CancellationToken cancellationToken = default)
        where T : class
    {
        return await SendForDataAsync<T>(method, uri, payload, null, cancellationToken);
    }

    private async Task<ApiResult> SendAsync(
        HttpMethod method,
        string uri,
        object? payload = default,
        Dictionary<string, string>? headers = default,
        CancellationToken cancellationToken = default)
    {
        if (_httpClient == null)
        {
            return ApiResult.ForFailure(HttpStatusCode.ServiceUnavailable, problem: ServiceUnavailableProblem());
        }

        var absoluteUri = await ToUri(uri);
        
        var request = new HttpRequestMessage(method, absoluteUri);
        TryAddPayload(request, payload);
        TryAddHeaders(request, headers);

        HttpResponseMessage response;

        try
        {
            response = await _httpClient.SendAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            return ApiResult.ForFailure(HttpStatusCode.ServiceUnavailable, problem: ServiceUnavailableProblem(ex));
        }

        return await ParseResponseAsync(response);
    }

    private async Task<ApiDataResult<T>> SendForDataAsync<T>(
        HttpMethod method,
        string uri,
        object? payload = default,
        Dictionary<string, string>? headers = default,
        CancellationToken cancellationToken = default)
        where T : class
    {
        if (_httpClient == null)
        {
            return ApiDataResult<T>.ForFailure(HttpStatusCode.ServiceUnavailable, problem: ServiceUnavailableProblem());
        }

        var absoluteUri = await ToUri(uri);
        
        var request = new HttpRequestMessage(method, absoluteUri);
        TryAddPayload(request, payload);
        TryAddHeaders(request, headers);

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.SendAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            return ApiDataResult<T>.ForFailure(HttpStatusCode.ServiceUnavailable,
                problem: ServiceUnavailableProblem(ex));
        }

        return await ParseDataResponseAsync<T>(response);
    }

    private static async Task<ApiResult> ParseResponseAsync(HttpResponseMessage response)
    {
        var statusCode = response.StatusCode;
        var headers = GetHeaders(response);

        if (response.IsSuccessStatusCode)
        {
            return ApiResult.ForSuccess(statusCode, headers);
        }

        var error = await TryParseProblemAsync(response);
        return ApiResult.ForFailure(statusCode, headers, error);
    }

    private static async Task<ApiDataResult<T>> ParseDataResponseAsync<T>(HttpResponseMessage response)
    {
        var statusCode = response.StatusCode;
        var headers = GetHeaders(response);

        if (!response.IsSuccessStatusCode)
        {
            var error = await TryParseProblemAsync(response);
            return ApiDataResult<T>.ForFailure(statusCode, headers, error);
        }

        var data = await GetPayloadAsync<T>(response);

        return ApiDataResult<T>.ForSuccess(data, statusCode, headers);
    }

    private static async Task<T?> GetPayloadAsync<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return Serializer.Deserialize<T>(content);
    }

    private static async Task<Problem?> TryParseProblemAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return string.IsNullOrEmpty(content) ? null : Serializer.Deserialize<Problem>(content);
    }

    private static Dictionary<string, string> GetHeaders(HttpResponseMessage response)
    {
        var headers = new Dictionary<string, string>();

        foreach (var (name, values) in response.Headers)
        {
            var value = values.FirstOrDefault();

            if (!string.IsNullOrEmpty(value))
            {
                headers.Add(name, value);
            }
        }

        return headers;
    }

    private static void TryAddPayload(HttpRequestMessage request, object? payload)
    {
        if (payload == null)
        {
            return;
        }

        if (payload is MultipartFormDataContent formDataContent)
        {
            request.Content = formDataContent;
        }
        else
        {
            var body = Serializer.Serialize(payload);
            request.Content = new StringContent(body, Encoding.UTF8, MediaTypeNames.Application.Json);
        }
    }

    private static void TryAddHeaders(HttpRequestMessage request, Dictionary<string, string>? headers)
    {
        if (headers == null)
        {
            return;
        }

        foreach (var header in headers)
        {
            request.Headers.Add(header.Key, header.Value);
        }
    }

    private static Problem ServiceUnavailableProblem(Exception? ex = null)
    {
        return new Problem
        {
            Code = "General.ServiceUnavailable",
            Description = ex?.Message ?? "Service is unavailable"
        };
    }

    private async Task<Uri?> ToUri(string path)
    {
        if (_endpointProvider == null)
        {
            return new Uri(path);
        }

        var endpoint = await _endpointProvider.GetApiEndpoint();

        try
        {
            var baseUri = new Uri(endpoint!);
            return new Uri(baseUri, path);
        }
        catch (Exception)
        {
            return null;
        }
    }
}