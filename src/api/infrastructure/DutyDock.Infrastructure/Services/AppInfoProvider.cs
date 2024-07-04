using DutyDock.Application.Common.Interfaces;
using DutyDock.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Options;
using Throw;

namespace DutyDock.Infrastructure.Services;

public class AppInfoProvider : IAppInfoProvider
{
    private const string Default = "Unknown";

    private readonly AppOptions _options;

    public AppInfoProvider(IOptions<AppOptions> options)
    {
        options.ThrowIfNull();
        _options = options.Value;
    }

    public string Name => _options.Name ?? Default;

    public string Version => _options.Version ?? Default;

    public string WebDomain => _options.Domain?.Web ?? Default;

    public string ApiDomain => _options.Domain?.Api ?? Default;
}