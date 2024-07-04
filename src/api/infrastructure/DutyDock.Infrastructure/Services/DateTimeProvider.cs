using DutyDock.Application.Common.Interfaces;
using DutyDock.Application.Common.Interfaces.Services;

namespace DutyDock.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}