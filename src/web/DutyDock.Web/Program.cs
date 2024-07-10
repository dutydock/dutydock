using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace DutyDock.Web;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        builder.RootComponents.Add<App>("#app");

        builder
            .AddOptions()
            .AddApiClient()
            .AddSecurity()
            .AddStorage()
            .AddLogging()
            .AddMud()
            .AddServices()
            .AddCore();

        await builder.Build().RunAsync();
    }
}