using System.Web;

namespace DutyDock.Api.Client.Common;

public sealed class QueryStringBuilder
{
    private readonly List<KeyValuePair<string, string>> _parameters = new();

    public static QueryStringBuilder New()
    {
        return new QueryStringBuilder();
    }

    public QueryStringBuilder Add(string name, string value, bool urlEncode = true)
    {
        if (urlEncode)
        {
            value = HttpUtility.UrlEncode(value);
        }

        _parameters.Add(new KeyValuePair<string, string>(name, value));

        return this;
    }

    public QueryStringBuilder Add(string name, bool? value)
    {
        return value == null ? this : Add(name, value.ToString()!, false);
    }

    public QueryStringBuilder Add(string name, int? value)
    {
        return value == null ? this : Add(name, value?.ToString()!, false);
    }

    public QueryStringBuilder Add(string name, IEnumerable<string> values)
    {
        var enumerable = values.ToList();

        if (enumerable.Count == 0)
        {
            return this;
        }

        foreach (var value in enumerable)
        {
            Add(name, value);
        }

        return this;
    }

    public string Build()
    {
        if (!_parameters.Any())
        {
            return string.Empty;
        }

        var parameters = _parameters
            .Where(valuePair => !string.IsNullOrEmpty(valuePair.Value))
            .Select(valuePair => $"{HttpUtility.UrlEncode(valuePair.Key)}={valuePair.Value}")
            .ToArray();

        return "?" + string.Join("&", parameters);
    }
}