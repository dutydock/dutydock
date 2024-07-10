namespace DutyDock.Api.Client.Common;

public interface IApiEndpointProvider
{
    Task<string?> GetApiEndpoint();
}