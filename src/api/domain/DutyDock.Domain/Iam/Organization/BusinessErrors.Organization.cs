using ErrorOr;

namespace DutyDock.Domain.Iam.Organization;

public static partial class BusinessErrors
{
    public static class Organization
    {
        public static readonly Error NameRequired =
            Error.Validation("Organization.NameRequired", "Name is required");

        public static readonly Error NameTooLong =
            Error.Validation("Organization.NameTooLong", "Name is too long");
    }
}