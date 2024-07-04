namespace DutyDock.Application.Common.Interfaces.Services;

public interface IEnvironmentProvider
{
    string Name { get; }
    
    bool IsLocal { get; }
    
    bool IsIntegration { get; }
    
    bool IsProduction { get; }
    
    bool IsVirtual { get; }
    
    bool IsHosted { get; }
}