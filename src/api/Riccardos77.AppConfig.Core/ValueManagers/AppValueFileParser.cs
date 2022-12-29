using HtmlAgilityPack;

namespace Riccardos77.AppConfig.ValueManagers;

public class AppValueFileParser
{
    public string ParseHtml(
        byte[] fileContent,
        string sectionId,
        Dictionary<string, string> inputTags)
    {
        var htmlDoc = new HtmlDocument();
        using var fileStream = new MemoryStream(fileContent);
        htmlDoc.Load(fileStream, true);

        var sectionTagValues = htmlDoc.DocumentNode
            .SelectNodes($"body/section[@data-section-id='{sectionId}']")
            .Select(h => CreateTagValue(h))
            .ToArray();

        TagValue[] filteredTagValues;
        if (inputTags.Count > 0)
        {
            filteredTagValues = inputTags.Aggregate(
                sectionTagValues,
                (acc, inputTag) => FilterElements(acc, inputTag));
        }
        else
        {
            filteredTagValues = sectionTagValues.Where(t => (t.Tags?.Count ?? 0) == 0).ToArray();
        }

        return filteredTagValues.FirstOrDefault()?.Value?.InnerHtml;

        static TagValue CreateTagValue(HtmlNode node)
        {
            return new TagValue(
                node.GetAttributes()
                    .Where(a => a.Name.StartsWith("data-") && a.Name != "data-section-id")
                    .ToDictionary(a => a.Name.Replace("data-", string.Empty), a => a.Value),
                node);
        }
    }

    private static TagValue[] FilterElements(TagValue[] tagValues, KeyValuePair<string, string> inputTag)
    {
        var inputTagKey = inputTag.Key.ToLower();
        var result = tagValues.Where(t => (t.Tags?.TryGetValue(inputTagKey, out var value) ?? false) && value.ToLower() == inputTag.Value.ToLower()).ToArray();
        if (result.Length > 0)
        {
            return result;
        }
        else
        {
            return tagValues.Where(t => !t.Tags?.ContainsKey(inputTagKey) ?? true).ToArray();
        }
    }

    internal record TagValue(Dictionary<string, string> Tags, HtmlNode Value);
}
