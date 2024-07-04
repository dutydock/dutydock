using ErrorOr;
using MediatR;

namespace DutyDock.Application.Common.Authorization;

public interface IAuthorizationHandler<in TRequest> : IRequestHandler<TRequest, ErrorOr<Success>>
    where TRequest : IRequest<ErrorOr<Success>>
{
}