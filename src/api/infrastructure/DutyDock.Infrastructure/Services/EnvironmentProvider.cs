using DutyDock.Application.Common.Interfaces;
using DutyDock.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Hosting;

namespace DutyDock.Infrastructure.Services;

public class EnvironmentProvider : IEnvironmentProvider
{
    private const string AspNetCoreEnvironmentVar = "ASPNETCORE_ENVIRONMENT";

    // Hosted environment, serving real users
    private const string EnvProduction = "Production";

    // Virtual environment, used for integration testing (CI)
    private const string EnvIntegration = "Integration";

    // Virtual environment, representing the app running on the developer machine
    private const string EnvLocal = "Local";

    private readonly string? _environmentName;

    public EnvironmentProvider(IHostEnvironment hostEnvironment)
    {
        _environmentName = hostEnvironment.EnvironmentName;
    }

    public string Name
    {
        get
        {
            if (!string.IsNullOrEmpty(_environmentName))
            {
                return _environmentName;
            }

            return Environment.GetEnvironmentVariable(AspNetCoreEnvironmentVar) ?? "Unknown";
        }
    }

    public bool IsLocal => IsEnvironment(EnvLocal);

    public bool IsIntegration => IsEnvironment(EnvIntegration);

    public bool IsProduction => IsEnvironment(EnvProduction);

    public bool IsVirtual => IsLocal || IsIntegration;

    public bool IsHosted => IsProduction;

    private bool IsEnvironment(string name)
    {
        return Name.Equals(name, StringComparison.InvariantCultureIgnoreCase);
    }
}