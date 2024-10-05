using ImagesDownloader.Core.Interfaces;

namespace ImagesDownloader.Core.Internal;

internal class LoggerConsole : ILogger
{
    const string format = "[{0:yyyy-MM-dd HH:mm:ss.ffff}] [{1}] {2}";

    public void Log(LogLevel logLevel, string message)
    {
        Console.ForegroundColor = logLevel switch
        {
            LogLevel.WARN => ConsoleColor.Yellow,
            LogLevel.FAIL => ConsoleColor.Red,
            _ => Console.ForegroundColor
        };
        Console.WriteLine(format, DateTimeOffset.Now, logLevel.ToString(), message);
        Console.ResetColor();
    }
}
