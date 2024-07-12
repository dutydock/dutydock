using ErrorOr;

namespace DutyDock.Domain.Scheduling.Roster;

public static class BusinessErrors
{
    public static class Roster
    {
        public static readonly Error FromAfterTill =
            Error.Validation("Roster.FromAfterTill", "From after till time");
    }
}