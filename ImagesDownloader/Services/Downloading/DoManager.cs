using ImagesDownloader.Interfaces;
using ImagesDownloader.Models;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ImagesDownloader.Services.Downloading;

internal class DoManager
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;

    public DoManager(IServiceProvider serviceProvider, ILogger logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public void TestRun()
    {
        List<DownloadItemCollection> all = [
            new([
                new(new Uri("https://yandex1.ru"), "yadir1"),
                new(new Uri("https://yandex2.ru"), "yadir2"),
                new(new Uri("https://vk.ru"), "vk"),
                new(new Uri("https://yandex3.ru"), "yadir3")
            ]),

            new([
                new(new Uri("https://yandex4.ru"), "yadir4"),
                new(new Uri("https://yandex5.ru"), "yadir5"),
                new(new Uri("https://yandex6.ru"), "yadir6"),
                new(new Uri("https://yandex7.ru"), "yadir7")
            ])
        ];
        all.Reverse();

        using var downloadClient = _serviceProvider.GetRequiredService<IDownloadClient>();
        var doPool = new DoPool(
            poolSize: 4,
            sleepTime: 2000,
            collections: all,
            downloadClient: downloadClient,
            CancellationToken.None);
        doPool.ItemDownloaded += (s, e) =>
        {
            string log = $"ItemDownloaded event triggered: {e.Item}";
            if (e.IsSuccess)
                _logger.LogInformation(log);
            else
                _logger.LogError() // TODO
        };
        doPool.Wait();
        doPool.Dispose();
    }
}
