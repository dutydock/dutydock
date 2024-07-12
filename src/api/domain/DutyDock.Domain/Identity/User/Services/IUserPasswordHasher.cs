namespace DutyDock.Domain.Identity.User.Services;

public interface IUserPasswordHasher
{
    string Hash(string text);

    bool Verify(string text, string hash);
}