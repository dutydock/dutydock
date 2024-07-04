using MediatR;

namespace DutyDock.Application.Common.Requests;

public interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, TResult> where TQuery : IQuery<TResult>
{
}