using System.Runtime.CompilerServices;

namespace ImagesDownloader.Interfaces;

internal enum LogType
{
    INFO, WARN, FAIL
}

internal interface ILogger
{
    void Log(LogType logType, string message, [CallerMemberName] string? place = null);
    void LogError(string message, [CallerMemberName] string? place = null);
    void LogError(Exception ex, [CallerMemberName] string? place = null);
}
