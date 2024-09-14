using ImagesDownloader.Interfaces;

namespace ImagesDownloader.Internal;

internal class LoggerDebug : ILogger
{
    const string format = "[{0:yyyy-MM-dd HH:mm:ss.ffff}] [{1}] {2}";

    public void Log(LogLevel logLevel, string message) 
        => System.Diagnostics.Debug.WriteLine(format, DateTimeOffset.Now, logLevel.ToString(), message);
}
