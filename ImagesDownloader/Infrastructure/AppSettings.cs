using System.IO;
using Newtonsoft.Json;

namespace ImagesDownloader.Infrastructure;

internal class AppSettings : IDisposable
{
    public static readonly AppSettings Instance;

    public List<string> XPathHistory { get; set; } = [];
    [JsonIgnore]
    public string LastXPath => XPathHistory.Count != 0 ? XPathHistory[^1] : string.Empty;
    public string LastSavePath { get; set; } = string.Empty;
    public string LastNameFormat { get; set; } = string.Empty;
    public int LastCollectionPoolSize { get; set; } = 1;
    public int LastDownloadingPoolSize { get; set; } = 1;
    public int LastThreadSleepTime { get; set; } = 0;

    static AppSettings()
    {
        if (File.Exists("appsettings.json"))
        {
            string data = File.ReadAllText("appsettings.json");
            Instance = JsonConvert.DeserializeObject<AppSettings>(data)
                ?? throw new InvalidOperationException("Can't read appsettings.json");
        }
        else
            Instance = new();
    }

    public void AddXPathSuggest(string xPath)
    {
        XPathHistory.Remove(xPath);
        if (XPathHistory.Count == 10)
            XPathHistory.RemoveAt(0);
        XPathHistory.Add(xPath);
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
