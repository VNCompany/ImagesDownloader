using System.Runtime.CompilerServices;

using ImagesDownloader.Interfaces;

namespace ImagesDownloader.Services;

internal class DebugLogger : ILogger
{
    private enum LogType
    {
        INFO, WARN, FAIL
    }


    const string format = "[{0:yyyy-MM-dd HH:mm:ss.ffff}] [{1}] {2}. {3}";
    const string exFormat = "Exception {0} thrown.\nMessage: {1}";


    public void Info(string message, string? place = null) 
    {

    }

    public void Warn(string message, string? place = null) 
    {

    }

    public void Error(string message, string? place = null) 
    {

    }

    public void Exception(Exception exception, string? place = null) 
    {

    }


    private void Log(LogType logType, string message, string? place)
    {
        if (place == null)
        {
            var trace = new System.Diagnostics.StackTrace();
            trace.GetFrame(0).Get
        }

        System.Diagnostics.Debug.WriteLine(format, DateTimeOffset.Now, logType.ToString(), place, message);
    }
}
