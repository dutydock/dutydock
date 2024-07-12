using DutyDock.Application.Common.Database;
using DutyDock.Infrastructure.Database.Cosmos.Data;
using DutyDock.Infrastructure.Database.Cosmos.Repositories.Common;
using DutyDock.Infrastructure.Database.Cosmos.Repositories.Entities.User.Specifications;

namespace DutyDock.Infrastructure.Database.Cosmos.Repositories.Entities.User;

internal class UserRepository : Repository<Domain.Identity.User.User>, IUserRepository
{
    public UserRepository(DataContainerContext context, ContainerProvider containerProvider) :
        base(context, containerProvider)
    {
    }

    public async Task<(Domain.Identity.User.User?, string?)> GetByEmailAddress(string emailAddress,
        CancellationToken cancellationToken = default)
    {
        var specification = new UserByEmailAddressSpecification(emailAddress);
        return await GetOneBySpecification(specification, cancellationToken: cancellationToken);
    }
}