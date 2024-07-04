using DutyDock.Domain.Common.Models.Events;
using MediatR;

namespace DutyDock.Application.Common.Events;

public interface IDomainEventHandler<in T> : INotificationHandler<T> where T : DomainEvent
{
}