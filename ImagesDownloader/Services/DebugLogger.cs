using System.Runtime.CompilerServices;

using ImagesDownloader.Interfaces;

namespace ImagesDownloader.Services;

internal class DebugLogger : ILogger
{
    const string format = "[{0:yyyy-MM-dd HH:mm:ss.ffff}] [{1}] {2}. {3}";
    const string exFormat = "Exception {0} thrown.\nMessage: {1}";

    public void Log(LogType logType, string message, [CallerMemberName] string? place = null)
    {
        System.Diagnostics.Debug.WriteLine(format, DateTimeOffset.Now, logType.ToString(), place, message);
    }

    public void LogError(string message, [CallerMemberName] string? place = null)
    {
        Log(LogType.FAIL, message, place);
    }

    public void LogError(Exception ex, [CallerMemberName] string? place = null)
    {
        LogError(string.Format(exFormat, ex.GetType().Name, ex.Message));
    }
}
