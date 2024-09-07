namespace ImagesDownloader.Interfaces;

enum LogLevel { INFO, WARN, FAIL }

internal interface ILogger
{
    void Log(LogLevel logLevel, string message);
}
