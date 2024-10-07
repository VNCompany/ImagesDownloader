using System.Text;

using ImagesDownloader.Core.Interfaces;

namespace ImagesDownloader.Core.Extensions;

public static class LoggerExtensions
{
    public static void Info(this ILogger logger, object state)
        => logger.Log(LogLevel.INFO, state.ToString() ?? string.Empty);

    public static void Info(this ILogger logger, string format, params object[] args)
        => logger.Log(LogLevel.INFO, string.Format(format, args));

    public static void Warn(this ILogger logger, object state)
        => logger.Log(LogLevel.WARN, state.ToString() ?? string.Empty);

    public static void Warn(this ILogger logger, string format, params object[] args)
        => logger.Log(LogLevel.WARN, string.Format(format, args));

    public static void Error(this ILogger logger, object state)
        => logger.Log(LogLevel.FAIL, state.ToString() ?? string.Empty);

    public static void Error(this ILogger logger, string format, params object[] args)
        => logger.Log(LogLevel.FAIL, string.Format(format, args));

    public static void Error(this ILogger logger, Exception exception, object? details = null)
    {
        StringBuilder sb = new StringBuilder($"{exception.GetType().Name} thrown. {exception.Message}.");
        if (details != null)
            sb.Append("\n\t" + details);
        logger.Log(LogLevel.FAIL, sb.ToString());
    }
}
