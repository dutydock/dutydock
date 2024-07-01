using DutyDock.Api.Common;
using DutyDock.Application;
using DutyDock.Infrastructure;

namespace DutyDock.Api;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddPresentation()
            .AddApplication()
            .AddInfrastructure(_configuration);
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseExceptionMiddleware();
        app.UseFileServer();
        app.UseSwaggerDocumentation();

        app.UseRouting();

        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(routeBuilder => { routeBuilder.MapControllers().RequireAuthorization(); });
    }
}