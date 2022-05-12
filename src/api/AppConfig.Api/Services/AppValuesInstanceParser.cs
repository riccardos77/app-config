using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Riccardos77.AppConfig.Api.Models;

namespace Riccardos77.AppConfig.Api.Services;

internal static class AppValuesInstanceParser
{
    public static string Parse(AppMetaschema metaschema, string valuesContent, string appIdentity, Dictionary<string, string> inputTags)
    {
        var jobj = JsonConvert.DeserializeObject<JObject>(valuesContent);
        var result = new Dictionary<string, object?>()
        {
            { "$schema", Constants.JsonSchemaId.AppValuesInstance(metaschema.AppName, appIdentity) },
        };

        foreach (var key in metaschema.Schemas.ForIdentity(appIdentity).Select(s => s.Key))
        {
            var tagValues = GetTagValues(jobj.SelectToken(key));

            TagValue[] filteredTagValues;
            if (inputTags.Count > 0)
            {
                filteredTagValues = inputTags.Aggregate(
                    tagValues,
                    (acc, inputTag) => FilterElements(acc, inputTag));
            }
            else
            {
                filteredTagValues = tagValues.Where(t => (t.Tags?.Count ?? 0) == 0).ToArray();
            }

            InsertIntoResult(key, filteredTagValues.FirstOrDefault()?.Value, result);
        }

        return JsonConvert.SerializeObject(result, Formatting.Indented);
    }

    private static TagValue[] GetTagValues(JToken? jToken)
    {
        if (jToken?.Type == JTokenType.Array)
        {
            return JsonConvert.DeserializeObject<TagValue[]>(jToken.ToString());
        }
        else
        {
            return new TagValue[] { new(new(), jToken) };
        }
    }

    private static TagValue[] FilterElements(TagValue[] tagValues, KeyValuePair<string, string> inputTag)
    {
        var result = tagValues.Where(t => (t.Tags?.TryGetValue(inputTag.Key, out var value) ?? false) && value.ToLower() == inputTag.Value.ToLower()).ToArray();
        if (result.Length > 0)
        {
            return result;
        }
        else
        {
            return tagValues.Where(t => !t.Tags?.ContainsKey(inputTag.Key) ?? true).ToArray();
        }
    }

    private static void InsertIntoResult(string key, JToken? value, Dictionary<string, object?> result)
    {
        var keyParts = key.Split(".");
        foreach (var keyPart in keyParts[0..^1])
        {
            if (!result.ContainsKey(keyPart))
            {
                result[keyPart] = new Dictionary<string, object>();
            }

            result = result[keyPart] as Dictionary<string, object>;
        }

        result.Add(keyParts[^1], value);
    }

    internal record TagValue(Dictionary<string, string> Tags, JToken? Value);
}
