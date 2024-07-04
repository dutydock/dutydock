namespace DutyDock.Application.Common.Interfaces.Notifications;

public interface IEmailSender
{
    Task<bool> SendSystem(string subject, string textBody, CancellationToken cancellationToken = default);
    
    Task<bool> SendUser(string userEmailAddress, string userName, string subject, string? htmlBody = null,
        string? textBody = null, CancellationToken cancellationToken = default);
}