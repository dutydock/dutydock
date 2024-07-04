namespace DutyDock.Application.Common.Interfaces.Services;

public interface IAppInfoProvider
{
    public string Name { get; }
    
    public string Version { get; }
    
    public string WebDomain { get; }
    
    public string ApiDomain { get; }
}