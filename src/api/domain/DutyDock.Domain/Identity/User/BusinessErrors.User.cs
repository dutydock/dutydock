using ErrorOr;

namespace DutyDock.Domain.Identity.User;

public static class BusinessErrors
{
    public static class User
    {
        public static readonly Error NameRequired =
            Error.Validation("User.NameRequired", "Name is required");
        
        public static readonly Error NameTooLong =
            Error.Validation("User.NameTooLong", "Name is too long");
        
        public static readonly Error EmailAddressRequired =
            Error.Validation("User.EmailAddressRequired", "Email address is required");

        public static readonly Error EmailAddressInvalid =
            Error.Validation("User.EmailAddressInvalid", "Email address has an invalid format");

        public static readonly Error EmailAddressTooLong =
            Error.Validation("User.EmailAddressTooLong", "Email address is too long");

        public static readonly Error PasswordRequired =
            Error.Validation("User.PasswordRequired", "Password is required");

        public static readonly Error PasswordTooShort =
            Error.Validation("User.PasswordTooShort", "Password is too short");

        public static readonly Error PasswordTooLong =
            Error.Validation("User.PasswordTooLong", "Password is too long");
        
        public static readonly Error UnknownCulture =
            Error.Validation("User.UnknownCulture", "Unknown culture");
        
        public static readonly Error UnknownTimeZone =
            Error.Validation("User.UnknownTimeZone", "Unknown time zone");
        
        public static readonly Error UnknownLanguage =
            Error.Validation("User.UnknownLanguage", "Unknown language");
        
        public static readonly Error InvalidMembershipStatus =
            Error.Validation("User.InvalidMembershipStatus", "Invalid membership status");

        public static readonly Error InvalidMembershipRole =
            Error.Validation("User.InvalidMembershipRole", "Invalid membership role");
        
        public static readonly Error MembershipExists =
            Error.Validation("User.MembershipExists", "Member was added before");
        
        public static readonly Error MembershipNonexistent =
            Error.Validation("User.MembershipNonexistent", "Member was not added");
        
        public static readonly Error OwnerMembershipTermination =
            Error.Validation("User.OwnerMembershipTermination", "Owner cannot terminate own membership");
        
        public static readonly Error MinimalAccount =
            Error.Validation("User.MinimalAccount", "Operation cannot be executed on a minimal account");
        
        public static readonly Error FullAccount =
            Error.Validation("User.FullAccount", "Operation cannot be executed on a full account");
        
        public static readonly Error MembershipInviteUnavailable =
            Error.Validation("User.MembershipInviteUnavailable", "Membership invite unavailable");
    }
}