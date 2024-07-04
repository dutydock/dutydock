namespace DutyDock.Domain.Common.Models.Events;

public interface IDomainEventEmitter<TEvent> where TEvent : DomainEvent
{
    void AddEvent(TEvent domainEvent);

    void RemoveEvent(TEvent domainEvent);

    void RemoveAllEvents();

    IReadOnlyList<TEvent> Events { get; }
}