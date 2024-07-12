using ErrorOr;

namespace DutyDock.Domain.Scheduling.Slot;

public static class BusinessErrors
{
    public static class Activity
    {
        public static readonly Error FromAfterTill =
            Error.Validation("Activity.FromAfterTill", "From after till time");
        
        public static readonly Error BeforeRosterFrom =
            Error.Validation("Activity.BeforeRosterFrom", "From before roster from");
        
        public static readonly Error AfterRosterTill =
            Error.Validation("Activity.AfterRosterTill", "Till after roster till");
    }
}