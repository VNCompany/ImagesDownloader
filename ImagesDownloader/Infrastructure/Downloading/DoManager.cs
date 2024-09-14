using ImagesDownloader.Interfaces;
using ImagesDownloader.Models;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ImagesDownloader.Infrastructure.Downloading;

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
                new(new Uri("https://vk.com"), "vk"),
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

        using var cancellationSource = new CancellationTokenSource();
        using var downloadClient = _serviceProvider.GetRequiredService<IDownloader>();
        var doPool = new DoPool(
            poolSize: 2,
            sleepTime: 500,
            collections: all,
            downloadClient: downloadClient,
            cancellationSource.Token);
        doPool.ItemDownloaded += (s, e) =>
        {
            string log = $"ItemDownloaded event triggered: {e.Item}";
            if (e.IsSuccess)
                _logger.LogInformation("Item downloaded: {0}", e.Item);
            else
                _logger.LogError(e.Exception!, e.Item);
        };

        Thread.Sleep(1000);
        cancellationSource.Cancel();

        doPool.Wait();
        doPool.Dispose();
    }
}
