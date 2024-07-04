using System.Text;
using DutyDock.Application.Common.Interfaces.Notifications;
using Microsoft.Extensions.Logging;
using Throw;

namespace DutyDock.Infrastructure.Notifications.Sms;

public class ConsoleSmsSender : ISmsSender
{
    private const string Separator = "*************************************************************";

    private readonly ILogger<ConsoleSmsSender> _logger;

    public ConsoleSmsSender(ILogger<ConsoleSmsSender> logger)
    {
        logger.ThrowIfNull();
        _logger = logger;
    }

    public Task<bool> Send(string phoneNumber, string body)
    {
        var builder = new StringBuilder();

        builder.AppendLine();
        builder.AppendLine(Separator);
        builder.AppendLine($"To:      {phoneNumber}");
        builder.AppendLine(Separator);
        builder.AppendLine(body);
        builder.AppendLine(Separator);
        builder.AppendLine();

        _logger.LogInformation("{Message}", builder.ToString());

        return Task.FromResult(true);
    }
}