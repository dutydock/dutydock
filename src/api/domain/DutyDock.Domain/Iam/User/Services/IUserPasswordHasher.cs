namespace DutyDock.Domain.Iam.User.Services;

public interface IUserPasswordHasher
{
    string Hash(string text);

    bool Verify(string text, string hash);
}