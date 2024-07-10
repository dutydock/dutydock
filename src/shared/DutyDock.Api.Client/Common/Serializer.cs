using DutyDock.Api.Contracts.Common;
using Newtonsoft.Json;

namespace DutyDock.Api.Client.Common;

public static class Serializer
{
    public static string Serialize(object obj, IEnumerable<JsonConverter>? converters = null)
    {
        return JsonConvert.SerializeObject(obj, JsonSettings.Get(converters));
    }

    public static T? Deserialize<T>(string obj, IEnumerable<JsonConverter>? converters = null)
    {
        return JsonConvert.DeserializeObject<T>(obj, JsonSettings.Get(converters));
    }
}