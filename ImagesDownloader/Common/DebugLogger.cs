using ImagesDownloader.Interfaces;

namespace ImagesDownloader.Common;

internal class DebugLogger : ILogger
{
    public static readonly DebugLogger Instance = new();

    public void Error(string source, string message)
    {
        Debug.WriteLine($"{DateTimeOffset.Now.ToString()} FAIL: {source}. {message}");
    }

    public void Dispose() { }
}
