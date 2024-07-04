using DutyDock.Application.Common.Interfaces.Notifications;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Throw;
using Twilio;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;

namespace DutyDock.Infrastructure.Notifications.Sms;

public class TwilioSmsSender : ISmsSender
{
    private readonly ILogger<TwilioSmsSender> _logger;
    private readonly SmsOptions _options;

    private readonly PhoneNumber _fromPhoneNumber;

    public TwilioSmsSender(ILogger<TwilioSmsSender> logger, IOptions<SmsOptions> options)
    {
        logger.ThrowIfNull();
        _logger = logger;

        options.ThrowIfNull();
        _options = options.Value;

        var accountSid = _options.Twilio?.AccountSid;
        accountSid.ThrowIfNull().IfEmpty();

        var authToken = _options.Twilio?.AuthToken;
        authToken.ThrowIfNull().IfEmpty();

        TwilioClient.Init(accountSid, authToken);

        var phoneNumber = _options.Twilio?.PhoneNumber;
        phoneNumber.ThrowIfNull().IfEmpty();

        _fromPhoneNumber = new PhoneNumber(phoneNumber);
    }

    public async Task<bool> Send(string phoneNumber, string body)
    {
        _logger.LogInformation("Sending sms to {PhoneNumber} ({Body})", phoneNumber, body);

        if (_options.UseSandbox)
        {
            return true;
        }

        var toPhoneNumber = new PhoneNumber(phoneNumber);

        try
        {
            var message = await MessageResource.CreateAsync(body: body, from: _fromPhoneNumber, to: toPhoneNumber);

            if (message.Status != MessageResource.StatusEnum.Queued)
            {
                _logger.LogError("Sending sms failed: {ErrorCode}, {ErrorMessage} ({Status})",
                    message.ErrorCode, message.ErrorMessage, message.Status);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sending sms failed: {Message}", ex.Message);
            return false;
        }

        return true;
    }
}