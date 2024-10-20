using System.Text.RegularExpressions;

namespace ImagesDownloader.Infrastructure;

internal class HtmlParser
{
    private readonly Regex _regex = new Regex("^(.+)/@([-_A-Za-z0-9]+)$");
    private readonly HtmlAgilityPack.HtmlDocument _doc;

    public HtmlParser(string html)
    {
        _doc = new HtmlAgilityPack.HtmlDocument();
        _doc.LoadHtml(html);
    }

    public IEnumerable<string> Parse(string xPath)
    {
        string? attr = null;
        Match m = _regex.Match(xPath);
        if (m.Success)
        {
            xPath = m.Groups[1].Value;
            attr = m.Groups[2].Value;
        }

        var nodes = _doc.DocumentNode.SelectNodes(xPath);
        if (nodes == null || nodes.Count == 0)
            return [];

        return attr == null
            ? nodes.Select(x => x.InnerHtml)
            : nodes.Select(x => x.GetAttributeValue(attr, string.Empty))
                   .Where(x => x != string.Empty);
    }
}
