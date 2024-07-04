using MediatR;

namespace DutyDock.Application.Common.Requests;

public interface ICommand : IRequest
{
}

public interface ICommand<out TResult> : IRequest<TResult>
{
}