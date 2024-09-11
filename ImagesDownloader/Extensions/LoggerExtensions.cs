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
            string.Format("Target: {0}. \n\tMessage: {1}", scope ?? "Application", state.ToString() ?? string.Empty));

    public static void LogError(
        this ILogger logger, 
        Exception exception, string? scope = null, object? details = null)
    {
        string log = string.Format("Target: {0}. \n\tException: {1}. \n\tMessage: {2}",
            scope ?? "Application",
            exception.GetType().Name,
            exception.Message);
        if (details != null)
            log += string.Concat(" \n\tDetails: " + details.ToString());
        logger.Log(LogLevel.FAIL, log);
    }
}
