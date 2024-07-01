using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace DutyDock.Api;

public static class Program
{
    private const int ExitOk = 0;
    private const int ExitError = 1;

    private const string LogTemplate =
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}";

    public static int Main(string[] args)
    {
        try
        {
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return ExitError;
        }

        return ExitOk;
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
            .UseSerilog((hostingContext, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration).Enrich.FromLogContext()
                    .WriteTo.Console(theme: ConsoleTheme.None, outputTemplate: LogTemplate);
            });
}