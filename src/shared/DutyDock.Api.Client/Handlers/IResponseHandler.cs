namespace DutyDock.Api.Client.Handlers;

public interface IResponseHandler
{
    Task Handle(HttpResponseMessage message);
}