using ErrorOr;
using DutyDock.Api.Contracts.Dto.Users;
using DutyDock.Application.Common.Authorization.Policies;
using DutyDock.Application.Common.Requests;

namespace DutyDock.Application.Identity.Users.Commands.SignIn;

public sealed record SignInUserCommand : ICommand<ErrorOr<AuthenticationDetails>>
{
    public string? EmailAddress { get; set; }

    public string? Password { get; set; }

    public bool IsPersisted { get; set; }
}

public sealed class SignInUserCommandAuthorizer : AnonymousAuthorizer<SignInUserCommand>
{
}