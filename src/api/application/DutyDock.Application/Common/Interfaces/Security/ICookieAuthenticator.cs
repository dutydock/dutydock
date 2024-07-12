using DutyDock.Domain.Identity.User;
using DutyDock.Domain.Identity.User.Entities;

namespace DutyDock.Application.Common.Interfaces.Security;

public interface ICookieAuthenticator
{
    Task Create(User user, Membership? membership = null, bool isPersistent = false);
    
    Task Destroy();
}