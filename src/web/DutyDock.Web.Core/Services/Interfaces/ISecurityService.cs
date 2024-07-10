namespace DutyDock.Web.Core.Services.Interfaces;

public interface ISecurityService
{
    Task<bool> UserCompliesWith(string policy);
}