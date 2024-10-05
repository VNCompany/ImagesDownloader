using ImagesDownloader.Core.Models;

namespace ImagesDownloader.Tests;

internal class Program
{
    static void Main(string[] args)
    {
        var dic = new DItemsCollection("Test Collection", [
            new(new Uri("https://yandex.ru/src/1"), "./1"),
            new(new Uri("https://yandex.ru/src/2"), "./2"),
            new(new Uri("https://vk.com/src/0"), "./0"),
            new(new Uri("https://yandex.ru/src/3"), "./4")
            ]);

        using SemaphoreSlim ss = new SemaphoreSlim(1);
        using CancellationTokenSource cts = new CancellationTokenSource();
        var task = dic.Download(ss, 1, cts.Token);
        Thread.Sleep(1000);
        cts.Cancel();
        task.Wait();
    }
}
