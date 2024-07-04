using DutyDock.Infrastructure.Notifications.Email;
using DutyDock.Infrastructure.Notifications.Sms;

namespace DutyDock.Infrastructure.Notifications;

public sealed record NotificationOptions
{
    public const string Section = "Notifications";

    public EmailOptions? Email { get; set; }
    
    public SmsOptions? Sms { get; set; }
}