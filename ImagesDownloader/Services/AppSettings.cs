using Newtonsoft.Json;
using System.IO;

using ImagesDownloader.Interfaces;

namespace ImagesDownloader.Services;

internal class AppSettings : IDisposable
{
    private readonly ILogger _logger;

    public List<string> XPathSuggests { get; set; } = [];
    [JsonIgnore]
    public string LastXPathSuggest => XPathSuggests.Count != 0 ? XPathSuggests[^1] : string.Empty;
    public string SavePathSuggest { get; set; } = string.Empty;
    public string NamePatternSuggest { get; set; } = string.Empty;
    public int MaxDownloadingItemsCount { get; set; } = 1;
    public int MaxDownloadingCollectionsCount { get; set; } = 1;

    public AppSettings(ILogger logger)
    {
        _logger = logger;
        
        if (File.Exists("appsettings.json"))
        {
            string data = File.ReadAllText("appsettings.json");
            JsonConvert.PopulateObject(data, this);
        }
    }

    public void AddXPathSuggest(string xPath)
    {
        XPathSuggests.Remove(xPath);
        if (XPathSuggests.Count == 10)
            XPathSuggests.RemoveAt(0);
        XPathSuggests.Add(xPath);
    }

    public void Save()
    {
        File.WriteAllText("appsettings.json", JsonConvert.SerializeObject(this));
    }

    public void Dispose()
    {
        Save();
    }
}
