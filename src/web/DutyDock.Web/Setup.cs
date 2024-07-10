using Blazored.LocalStorage;
using DutyDock.Api.Client.Common;
using DutyDock.Api.Client.Handlers;
using DutyDock.Api.Contracts.Dto.Organizations;
using DutyDock.Api.Web.Client;
using DutyDock.Web.Core;
using DutyDock.Web.Core.Models.Security;
using DutyDock.Web.Core.Services.Interfaces;
using DutyDock.Web.Shared.Http;
using DutyDock.Web.Shared.Security;
using DutyDock.Web.Shared.Security.AuthorizationHandlers;
using DutyDock.Web.Shared.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using MudExtensions.Services;

namespace DutyDock.Web;

public static class Setup
{
    public static WebAssemblyHostBuilder AddOptions(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddOptions();
        return builder;
    }

    public static WebAssemblyHostBuilder AddApiClient(this WebAssemblyHostBuilder builder)
    {
        var apiEndpoint = builder.Configuration["Api:Endpoint"] ?? "https://localhost:5001/";
        var apiTimeout = int.Parse(builder.Configuration["Api:TimeoutInSeconds"] ?? "15");

        var environment = builder.HostEnvironment.Environment;

        Console.WriteLine($"Environment: {environment}");
        Console.WriteLine($"Api Endpoint: {apiEndpoint}");

        builder.Services.AddScoped<IResponseHandler, GlobalResponseHandler>();

        builder.Services.AddScoped<IWebApiClient>(provider =>
            new WebApiClientBuilder()
                .WithApiEndpoint(new ApiEndpointProvider(apiEndpoint))
                .WithRequestTimeout(TimeSpan.FromSeconds(apiTimeout))
                .WithCookieHandling()
                .WithResponseHandler(provider.GetService<IResponseHandler>)
                .Build()
        );

        return builder;
    }

    public static WebAssemblyHostBuilder AddSecurity(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddScoped<IAuthorizationHandler, IsValidatedAuthorizationHandler>();
        builder.Services.AddScoped<IAuthorizationHandler, HasRoleAuthorizationHandler>();
        builder.Services.AddScoped<IAuthorizationHandler, IsOwnerAuthorizationHandler>();

        builder.Services.AddAuthorizationCore(options =>
        {
            options.AddPolicy(Policies.Verified, policy => policy.AddRequirements(new IsValidatedRequirement()));
            options.AddPolicy(Policies.User, policy =>
            {
                policy.AddRequirements(new IsValidatedRequirement());
                policy.AddRequirements(new HasRoleRequirement(Role.User));
            });
            options.AddPolicy(Policies.Manager, policy =>
            {
                policy.AddRequirements(new IsValidatedRequirement());
                policy.AddRequirements(new HasRoleRequirement(Role.Manager));
            });
            options.AddPolicy(Policies.Admin, policy =>
            {
                policy.AddRequirements(new IsValidatedRequirement());
                policy.AddRequirements(new HasRoleRequirement(Role.Admin));
            });
            options.AddPolicy(Policies.Owner, policy =>
            {
                policy.AddRequirements(new IsValidatedRequirement());
                policy.AddRequirements(new IsOwnerRequirement());
            });
        });

        builder.Services.AddScoped<AuthenticationStateProvider, UserAuthenticationStateProvider>();
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        builder.Services.AddScoped<ISecurityService, SecurityService>();

        return builder;
    }

    public static WebAssemblyHostBuilder AddStorage(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddScoped<IStorageService, StorageService>();

        return builder;
    }

    public static WebAssemblyHostBuilder AddLogging(this WebAssemblyHostBuilder builder)
    {
        builder.Logging.AddConfiguration(
            builder.Configuration.GetSection("Logging"));

        return builder;
    }

    public static WebAssemblyHostBuilder AddMud(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddMudServices(configuration =>
        {
            configuration.SnackbarConfiguration.MaximumOpacity = 100;
            configuration.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
            configuration.SnackbarConfiguration.NewestOnTop = false;
            configuration.SnackbarConfiguration.PreventDuplicates = false;
            configuration.SnackbarConfiguration.MaxDisplayedSnackbars = 5;
        });
        builder.Services.AddMudExtensions();

        //builder.Services.AddScoped<IFlagManager, FlagManager>();
        //builder.Services.AddScoped<IDialogManager, DialogManager>();

        return builder;
    }

    public static WebAssemblyHostBuilder AddServices(this WebAssemblyHostBuilder builder)
    {
        return builder;
    }

    public static WebAssemblyHostBuilder AddCore(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddViewModels();
        builder.Services.AddCoreServices();

        return builder;
    }
}