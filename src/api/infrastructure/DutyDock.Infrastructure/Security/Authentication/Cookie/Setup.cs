using System.Net;
using DutyDock.Application.Common.Interfaces.Security;
using DutyDock.Application.Common.Interfaces.Services;
using DutyDock.Infrastructure.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Throw;

namespace DutyDock.Infrastructure.Security.Authentication.Cookie;

public static class Setup
{
    public static IServiceCollection AddCookieAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.ThrowIfNull();
        configuration.ThrowIfNull();

        const string sectionName = $"{SecurityOptions.Section}:{AuthenticationOptions.Section}:{CookieOptions.Section}";
        services.ConfigureAndValidate<CookieOptions>(sectionName, configuration);

        services.AddScoped<ICookieAuthenticator, CookieAuthenticator>();

        var options = configuration.GetSection(sectionName).Get<CookieOptions>();
        options.ThrowIfNull();

        var environment = services.BuildServiceProvider().GetRequiredService<IEnvironmentProvider>();
        environment.ThrowIfNull();

        // CookieAuthenticationDefaults.AuthenticationScheme
        services.AddAuthentication()
            .AddCookie(cookieOptions =>
            {
                // See also:
                // - https://developer.mozilla.org/en-US/docs/Web/HTTP/Cookies#__secure-
                // - https://developer.mozilla.org/en-US/docs/Web/HTTP/Cookies#__host-
                cookieOptions.Cookie.Name = string.IsNullOrEmpty(options.Name) ? "Auth" : options.Name;

                cookieOptions.Cookie.IsEssential = true;

                cookieOptions.Cookie.HttpOnly = true;

                // When using a virtual environment, the 'secure' flag is not required
                cookieOptions.Cookie.SecurePolicy =
                    environment.IsVirtual ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;

                cookieOptions.Cookie.Path = "/";

                if (!environment.IsVirtual)
                {
                    // When working locally, the domain (localhost) must not be included
                    cookieOptions.Cookie.Domain = options.Domain;
                }

                // SameSite policy is very important as a defence in preventing XSRF attacks,
                // see https://scotthelme.co.uk/csrf-is-dead/
                // When using a virtual environment, allow cross-origin cookie request
                var sameSiteMode = options.UseStrict ? SameSiteMode.Strict : SameSiteMode.Lax;
                cookieOptions.Cookie.SameSite = environment.IsVirtual ? SameSiteMode.None : sameSiteMode;

                cookieOptions.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return Task.CompletedTask;
                };

                cookieOptions.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return Task.CompletedTask;
                };
            });

        return services;
    }
}