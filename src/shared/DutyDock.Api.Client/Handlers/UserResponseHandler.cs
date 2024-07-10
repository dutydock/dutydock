namespace DutyDock.Api.Client.Handlers;

internal class UserResponseHandler : DelegatingHandler
{
    private readonly Func<IResponseHandler?> _handlerProvider;
    
    public UserResponseHandler(Func<IResponseHandler?> handlerProvider)
    {
        _handlerProvider = handlerProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        var handler = _handlerProvider();

        if (handler != null)
        {
            await handler.Handle(response);
        }

        return response;
    }
}