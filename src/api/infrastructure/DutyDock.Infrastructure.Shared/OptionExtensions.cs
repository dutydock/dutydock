using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DutyDock.Infrastructure.Shared;

public static class OptionExtensions
{
    public static void ConfigureAndValidate<T>(this IServiceCollection serviceCollection,
        string sectionName,
        IConfiguration configuration)
        where T : class, new()
    {
        serviceCollection.Configure<T>(configuration.GetSection(sectionName));

        using var scope = serviceCollection.BuildServiceProvider().CreateScope();

        var options = scope.ServiceProvider.GetRequiredService<IOptions<T>>();
        var optionsValue = options.Value;
        var configErrors = ValidationErrors(optionsValue).ToArray();

        if (configErrors.Length == 0)
        {
            return;
        }

        var aggregatedErrors = string.Join(",", configErrors);
        var count = configErrors.Length;
        var configType = typeof(T).FullName;

        throw new ApplicationException($"{configType} configuration has {count} error(s): {aggregatedErrors}");
    }

    private static IEnumerable<string> ValidationErrors(object obj)
    {
        var context = new ValidationContext(obj, serviceProvider: null, items: null);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(obj, context, results, true);
        foreach (var validationResult in results)
        {
            if (validationResult.ErrorMessage == null)
            {
                continue;
            }

            yield return validationResult.ErrorMessage;
        }
    }
}