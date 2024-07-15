using ImagesDownloader.Interfaces;
using System.IO;
using System.Text.Json;

namespace ImagesDownloader.Common;

internal class History : IDisposable
{
    private static readonly string _historyPath = Path.Combine(Environment.CurrentDirectory, "history.json");

    public List<string> XPaths { get; set; } = [];

    public string LastXPath { get; set; } = string.Empty;

    public string LastSavePath { get; set; } = string.Empty;

    public string LastNamePattern { get; set; } = string.Empty;

    public static History Load(ILogger logger)
    {
        try
        {
            return JsonSerializer.Deserialize<History>(File.ReadAllText(_historyPath))
                ?? throw new Exception("Serializer returned null");
        }
        catch (Exception ex)
        {
            logger.Error("History.Load", $"Failed load history file: {ex.Message}");
            return new History();
        }
    }

    public void AddXPathToList(string xPath)
    {
        XPaths.Remove(xPath);
        if (XPaths.Count >= 10)
            XPaths.RemoveAt(0);
        XPaths.Add(xPath);
        LastXPath = xPath;
    }

    public void Save()
    {
        File.WriteAllText(_historyPath, JsonSerializer.Serialize(this));
    }

    public void Dispose()
    {
        Save();
    }
}
