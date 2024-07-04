using MediatR;

namespace DutyDock.Application.Common.Requests;

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand> where TCommand : ICommand, IRequest<Unit>
{
}

public interface ICommandHandler<in TCommand, TResult> : IRequestHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
}