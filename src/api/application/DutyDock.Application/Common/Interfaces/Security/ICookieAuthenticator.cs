using DutyDock.Domain.Iam.User;
using DutyDock.Domain.Iam.User.Entities;

namespace DutyDock.Application.Common.Interfaces.Security;

public interface ICookieAuthenticator
{
    Task Create(User user, Membership? membership = null, bool isPersistent = false);
    
    Task Destroy();
}