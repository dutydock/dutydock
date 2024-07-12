using DutyDock.Infrastructure.Database.Common;
using DutyDock.Infrastructure.Database.Common.Specifications;

namespace DutyDock.Infrastructure.Database.Cosmos.Repositories.Entities.User.Specifications;

public class UserByEmailAddressSpecification : Specification<Domain.Identity.User.User>
{
    public UserByEmailAddressSpecification(string emailAddress) : 
        base(dataObject => dataObject.Data.EmailAddress.Value == emailAddress)
    {
    }
}