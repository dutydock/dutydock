namespace DutyDock.Api.Client.Common;

public interface IBearerTokenProvider
{
    Task<string?> GetToken();
}