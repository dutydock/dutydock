using ErrorOr;
using MediatR;

namespace DutyDock.Application.Common.Authorization;

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    private readonly IEnumerable<IAuthorizer<TRequest>> _authorizers;
    private readonly ISender _sender;

    public AuthorizationBehavior(IEnumerable<IAuthorizer<TRequest>> authorizers, ISender sender)
    {
        _authorizers = authorizers;
        _sender = sender;
    }

    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requirements = new List<IAuthorizationRequirement>();

        foreach (var authorizer in _authorizers)
        {
            authorizer.BuildPolicy(request);
            requirements.AddRange(authorizer.Requirements);
        }

        foreach (var requirement in requirements.Distinct())
        {
            var result = await _sender.Send(requirement, cancellationToken);

            if (result.IsError)
            {
                return (dynamic)result.Errors;
            }
        }

        return await next();
    }
}