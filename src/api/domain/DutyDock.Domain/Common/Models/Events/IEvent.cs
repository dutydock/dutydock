using MediatR;

namespace DutyDock.Domain.Common.Models.Events;

public interface IEvent : INotification
{
    string Id { get; }
    
    string Action { get; }
    
    DateTime CreatedAt { get; }
}