using System.ComponentModel.DataAnnotations;

namespace DutyDock.Infrastructure.Notifications.Sms;

public class SmsOptions
{
    public const string Section = "Sms";
    
    public enum SmsSender
    {
        Twilio,
        Console
    }
    
    public class TwilioOptions
    {
        [Required] public string? AccountSid { get; set; }
        
        [Required] public string? AuthToken { get; set; }

        [Required] public string? PhoneNumber { get; set; }
    }
    
    [Required] public SmsSender Sender { get; set; }
    
    [Required] public bool UseSandbox { get; set; }
    
    public TwilioOptions? Twilio { get; set; }
}