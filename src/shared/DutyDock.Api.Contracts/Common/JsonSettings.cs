using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace DutyDock.Api.Contracts.Common;

public static class JsonSettings
{
    public static readonly CultureInfo DefaultCulture = CultureInfo.InvariantCulture;
    public const string Iso8601DateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ";

    private static JsonSerializerSettings Create()
    {
        var jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy
                {
                    ProcessDictionaryKeys = false
                }
            },
            Culture = DefaultCulture,
            DateFormatString = Iso8601DateTimeFormat,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Include
        };

        jsonSettings.Converters.Add(new StringEnumConverter());

        return jsonSettings;
    }

    public static JsonSerializerSettings Get(IEnumerable<JsonConverter>? converters = null)
    {
        var settings = Create();

        if (converters == null)
        {
            return settings;
        }

        foreach (var converter in converters)
        {
            settings.Converters.Add(converter);
        }

        return settings;
    }
}