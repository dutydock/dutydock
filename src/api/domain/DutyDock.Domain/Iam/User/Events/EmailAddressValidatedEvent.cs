namespace DutyDock.Domain.Iam.User.Events;

public sealed class EmailAddressValidatedEvent : UserEvent
{
    public EmailAddressValidatedEvent()
    {
    }
    
    public EmailAddressValidatedEvent(string userId) : 
        base(userId, nameof(EmailAddressValidatedEvent))
    {
    }
}