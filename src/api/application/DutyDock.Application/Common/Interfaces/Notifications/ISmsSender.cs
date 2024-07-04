namespace DutyDock.Application.Common.Interfaces.Notifications;

public interface ISmsSender
{
    Task<bool> Send(string phoneNumber, string body);
}