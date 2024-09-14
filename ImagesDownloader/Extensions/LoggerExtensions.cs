using System.Text;

using ImagesDownloader.Interfaces;

namespace ImagesDownloader.Extensions;

internal static class LoggerExtensions
{
    public static void LogInformation(this ILogger logger, object state)
        => logger.Log(LogLevel.INFO, state.ToString() ?? string.Empty);

    public static void LogInformation(this ILogger logger, string format, params object[] args)
        => logger.Log(LogLevel.INFO, string.Format(format, args));

    public static void LogWarning(this ILogger logger, object state)
        => logger.Log(LogLevel.WARN, state.ToString() ?? string.Empty);

    public static void LogWarning(this ILogger logger, string format, params object[] args)
        => logger.Log(LogLevel.WARN, string.Format(format, args));

    public static void LogError(this ILogger logger, object state)
        => logger.Log(LogLevel.FAIL, state.ToString() ?? string.Empty);

    public static void LogError(this ILogger logger, string format, params object[] args)
        => logger.Log(LogLevel.FAIL, string.Format(format, args));

    public static void LogError(this ILogger logger, Exception exception, object? details = null)
    {
        StringBuilder sb = new StringBuilder($"{exception.GetType().Name} thrown. {exception.Message}.");
        if (details != null)
            sb.Append("\n\t" + details);
        logger.Log(LogLevel.FAIL, sb.ToString());
    }
}
