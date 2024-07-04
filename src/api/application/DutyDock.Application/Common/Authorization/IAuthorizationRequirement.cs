using ErrorOr;
using MediatR;

namespace DutyDock.Application.Common.Authorization;

public interface IAuthorizationRequirement : IRequest<ErrorOr<Success>>
{
}