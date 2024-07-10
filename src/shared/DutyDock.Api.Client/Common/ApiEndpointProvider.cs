namespace DutyDock.Api.Client.Common;

public class ApiEndpointProvider : IApiEndpointProvider
{
    private readonly string? _endpoint;
    
    public ApiEndpointProvider(string? endpoint)
    {
        _endpoint = endpoint;
    }

    public Task<string?> GetApiEndpoint()
    {
        return Task.FromResult(_endpoint);
    }
}