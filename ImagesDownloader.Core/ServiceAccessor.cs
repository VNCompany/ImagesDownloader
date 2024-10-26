using Microsoft.Extensions.DependencyInjection;

using ImagesDownloader.Core.Services;
using ImagesDownloader.Core.Interfaces;

namespace ImagesDownloader.Core;

public static class ServiceAccessor
{
    private static readonly ServiceProvider _sp;

    static ServiceAccessor()
    {
        var sc = new ServiceCollection();
        ConfigureServices(sc);
        _sp = sc.BuildServiceProvider();
    }

    public static T GetRequiredService<T>() where T : notnull => _sp.GetRequiredService<T>();

    public static IServiceScope CreateScope() => _sp.CreateScope();

    public static void Close() => _sp.Dispose();

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(provider => AppConfig.CreateInstance());
        //services.AddSingleton<ILogger, LoggerConsole>();
        services.AddSingleton<ILogger, LoggerDebug>();

        services.AddTransient<IDownloader, DownloadClientTest>();
    }
}
