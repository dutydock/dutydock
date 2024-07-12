using ErrorOr;

namespace DutyDock.Application.Identity.Users.Common;

public static class ApplicationErrors
{
    public static class User
    {
        public static readonly Error EmailAddressInUse = 
            Error.Validation("User.EmailAddressInUse", "Email address is already in use");
        
        public static readonly Error InvalidCredentials = 
            Error.Validation("User.InvalidCredentials", "Provided credentials are invalid");
        
        public static readonly Error InvalidEmailAddressConfirmationCode = 
            Error.Validation("User.InvalidEmailAddressConfirmationCode", "Provided email address confirmation code is invalid");

        public static readonly Error PasswordResetFailed = 
            Error.Validation("User.PasswordResetFailed", "Password reset failed");
        
        public static readonly Error InvalidInvitation = 
            Error.Validation("User.InvalidInvitation", "Provided invitation is invalid");
    }
}