namespace ImagesDownloader.Core.Interfaces;

public enum LogLevel { INFO, WARN, FAIL }

public interface ILogger
{
    void Log(LogLevel logLevel, string message);
}
