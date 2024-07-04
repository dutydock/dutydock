using System.ComponentModel.DataAnnotations;

namespace DutyDock.Infrastructure.Notifications.Email;

public sealed record EmailOptions
{
    public const string Section = "Email";

    public enum EmailSender
    {
        SendGrid,
        Console,
        Smtp
    }

    public class SmtpOptions
    {
        [Required] public string? Server { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }

        public uint Port { get; set; } = 587;
    }

    public class SendGridOptions
    {
        [Required] public string? ApiKey { get; set; }
    }

    [Required] public EmailSender Sender { get; set; }

    [Required] [EmailAddress] public string? EmailAddress { get; set; }

    [Required] public string? Name { get; set; }

    [Required] public bool UseSandbox { get; set; }

    public SendGridOptions? SendGrid { get; set; }

    public SmtpOptions? Smtp { get; set; }
}