using MediatR;

namespace DutyDock.Application.Common.Requests;

public interface IQuery<out TResult> : IRequest<TResult>
{
}