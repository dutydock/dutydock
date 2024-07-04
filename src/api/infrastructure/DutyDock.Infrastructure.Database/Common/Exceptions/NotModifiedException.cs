namespace DutyDock.Infrastructure.Database.Common.Exceptions;

public class NotModifiedException : Exception
{
    public NotModifiedException()
    {
    }

    public NotModifiedException(string message) : base(message)
    {
    }

    public NotModifiedException(string message, Exception inner) : base(message, inner)
    {
    }
}