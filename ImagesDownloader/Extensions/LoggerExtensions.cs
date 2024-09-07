using System.Text;

using ImagesDownloader.Interfaces;

namespace ImagesDownloader.Extensions;

internal static class LoggerExtensions
{
    public static void LogInformation(this ILogger logger, object state)
        => logger.Log(LogLevel.INFO, state.ToString() ?? string.Empty);

    public static void LogWarning(this ILogger logger, object state)
        => logger.Log(LogLevel.WARN, state.ToString() ?? string.Empty);

    public static void LogError(this ILogger logger, object state, string? scope = null)
        => logger.Log(
            LogLevel.FAIL,
            scope == null ? (state.ToString() ?? string.Empty) 
                          : string.Format("Source: {0}. Message: {1}", scope, state));

    public static void LogError(this ILogger logger, Exception exception, string? scope = null)
    {
        var sb = new StringBuilder();
        if (scope != null)
            sb.Append(string.Concat("Source: ", scope, " "));
        sb.Append(string.Format("Exception: {0}. Message: {1}", exception.GetType().Name, exception.Message));
        logger.Log(LogLevel.FAIL, sb.ToString());
    }
}
