using ImagesDownloader.Interfaces;

namespace ImagesDownloader.Services;

internal class DebugLogger : ILogger
{
    public void LogError(string message)
    {
        System.Diagnostics.Debug.WriteLine(message);
    }
}
