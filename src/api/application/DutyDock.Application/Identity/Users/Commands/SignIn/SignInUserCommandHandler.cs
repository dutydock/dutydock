using ErrorOr;
using DutyDock.Api.Contracts.Dto.Users;
using DutyDock.Application.Common.Database;
using DutyDock.Application.Common.Interfaces.Security;
using DutyDock.Application.Common.Requests;
using DutyDock.Application.Identity.Users.Common;
using DutyDock.Domain.Identity.Organization;
using DutyDock.Domain.Identity.User.Services;
using DutyDock.Domain.Identity.User.ValueObjects;

namespace DutyDock.Application.Identity.Users.Commands.SignIn;

public class SignInUserCommandHandler : ICommandHandler<SignInUserCommand, ErrorOr<AuthenticationDetails>>
{
    private readonly IUserRepository _userRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserPasswordHasher _passwordHasher;
    private readonly ICookieAuthenticator _cookieAuthenticator;

    public SignInUserCommandHandler(
        IUserRepository userRepository,
        IOrganizationRepository organizationRepository,
        IUserPasswordHasher passwordHasher,
        ICookieAuthenticator cookieAuthenticator)
    {
        _userRepository = userRepository;
        _organizationRepository = organizationRepository;
        _passwordHasher = passwordHasher;
        _cookieAuthenticator = cookieAuthenticator;
    }

    public async Task<ErrorOr<AuthenticationDetails>> Handle(SignInUserCommand command,
        CancellationToken cancellationToken)
    {
        // Input validation
        var password = command.Password ?? string.Empty;

        var emailAddressResult = EmailAddress.Create(command.EmailAddress!);

        if (emailAddressResult.IsError)
        {
            return ApplicationErrors.User.InvalidCredentials;
        }

        var emailAddress = emailAddressResult.Value;

        // Validate user
        var (existingUser, _) = await _userRepository.GetByEmailAddress(emailAddress.Value, cancellationToken);

        if (existingUser == null || existingUser.IsMinimalAccount())
        {
            return ApplicationErrors.User.InvalidCredentials;
        }

        // Validate password
        var passwordHash = existingUser.Password?.HashedValue;

        if (passwordHash == null || !_passwordHasher.Verify(password, passwordHash))
        {
            return ApplicationErrors.User.InvalidCredentials;
        }

        // Get default membership
        var defaultMembership = existingUser.GetDefaultMembership();
        Organization? organization = null;

        if (defaultMembership != null)
        {
            var defaultOrganizationId = defaultMembership.OrganizationId;
            (organization, _) = await _organizationRepository.GetById(defaultOrganizationId, cancellationToken);
        }

        // Sign in
        await _cookieAuthenticator.Create(existingUser, defaultMembership, command.IsPersisted);

        return existingUser.ToAuthenticationDetails(organization, defaultMembership);
    }
}