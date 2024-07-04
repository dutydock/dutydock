namespace DutyDock.Domain.Iam.User.Events;

public sealed class UserDeletedEvent : UserEvent
{
    public UserDeletedEvent()
    {
    }

    public UserDeletedEvent(string userId) : base(userId, nameof(UserDeletedEvent))
    {
    }
}