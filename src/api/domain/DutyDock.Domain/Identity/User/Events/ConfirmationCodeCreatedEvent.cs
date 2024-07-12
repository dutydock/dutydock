namespace DutyDock.Domain.Identity.User.Events;

public sealed class ConfirmationCodeCreatedEvent : UserEvent
{
    public ConfirmationCodeCreatedEvent()
    {
    }

    public ConfirmationCodeCreatedEvent(string userId) : 
        base(userId, nameof(ConfirmationCodeCreatedEvent))
    {
    }
}