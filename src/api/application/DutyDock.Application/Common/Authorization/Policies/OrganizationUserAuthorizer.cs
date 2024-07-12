using DutyDock.Application.Common.Authorization.Requirements;
using DutyDock.Domain.Identity.User.Enums;

namespace DutyDock.Application.Common.Authorization.Policies;

public abstract class OrganizationUserAuthorizer<TRequest> : Authorizer<TRequest>
{
    public override void BuildPolicy(TRequest request)
    {
        UseRequirement(new MustBeValidatedRequirement());
        UseRequirement(new MustHaveUnchangedUserSecurityStampRequirement());
        UseRequirement(new MustHaveUnchangedMembershipSecurityStampRequirement());
        UseRequirement(new MustBeActiveMemberRequirement());
        UseRequirement(new MustHaveMembershipRoleRequirement
        {
            Role = Role.User
        });
    }
}