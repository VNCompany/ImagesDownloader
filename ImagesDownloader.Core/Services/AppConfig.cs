using Newtonsoft.Json;

namespace ImagesDownloader.Core.Services;

public class AppConfig : IDisposable
{
    const string filePath = "./appconfig.json";

    public List<string> XPathSuggestsList { get; set; } = [];

    [JsonIgnore]
    public string XPathLastSuggest => XPathSuggestsList.LastOrDefault() ?? string.Empty;

    public string SavePathSuggest {  get; set; } = string.Empty;

    public void AddXPathSuggest(string value)
    {
        XPathSuggestsList.Remove(value);
        if (XPathSuggestsList.Count >= 10)
            XPathSuggestsList.RemoveAt(0);
        XPathSuggestsList.Add(value);
    }

    public void Dispose()
    {
        File.WriteAllText(filePath, JsonConvert.SerializeObject(this));
        GC.SuppressFinalize(this);
    }

    public static AppConfig CreateInstance()
    {
        if (File.Exists("./appconfig.json"))
            return JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(filePath)) ?? new();

        return new();
    }
}
