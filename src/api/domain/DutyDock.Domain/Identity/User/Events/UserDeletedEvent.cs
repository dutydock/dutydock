namespace DutyDock.Domain.Identity.User.Events;

public sealed class UserDeletedEvent : UserEvent
{
    public UserDeletedEvent()
    {
    }

    public UserDeletedEvent(string userId) : base(userId, nameof(UserDeletedEvent))
    {
    }
}