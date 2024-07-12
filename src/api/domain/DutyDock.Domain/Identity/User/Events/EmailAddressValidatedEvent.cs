namespace DutyDock.Domain.Identity.User.Events;

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