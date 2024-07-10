using DutyDock.Api.Web.Client.Users;

namespace DutyDock.Api.Web.Client;

public interface IWebApiClient
{
    IUsersService Users { get; }
}