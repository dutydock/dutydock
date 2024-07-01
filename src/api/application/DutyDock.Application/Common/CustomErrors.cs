using ErrorOr;

namespace DutyDock.Application.Common;

public static class CustomErrorType
{
    public const int Forbidden = 20;
}

public static class CustomError
{
    public static Error Forbidden(
        string code = "General.Forbidden",
        string description = "Not enough permissions") => Error.Custom(
        type: CustomErrorType.Forbidden,
        code: code,
        description: description);
}