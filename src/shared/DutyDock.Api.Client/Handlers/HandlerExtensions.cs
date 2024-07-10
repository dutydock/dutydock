namespace DutyDock.Api.Client.Handlers;

public static class HandlerExtensions
{
    public static DelegatingHandler DecorateWith(this HttpMessageHandler clientHandler, DelegatingHandler outerHandler)
    {
        outerHandler.InnerHandler = clientHandler;
        return outerHandler;
    }

    public static DelegatingHandler DecorateWith(this DelegatingHandler innerHandler, DelegatingHandler outerHandler)
    {
        outerHandler.InnerHandler = innerHandler;
        return outerHandler;
    }
}