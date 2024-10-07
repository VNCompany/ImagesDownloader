using ImagesDownloader.Core.Factories;
using ImagesDownloader.Core.Extensions;
using ImagesDownloader.Core.Models;

namespace ImagesDownloader.Tests;

internal class Program
{
    static void Main(string[] args)
    {
        const int collectionsCount = 10;
        var rnd = new Random();
        var userName = Faker.Name.First();

        var cc = new DItemsCollection[collectionsCount];
        for (int i = 0; i < collectionsCount; i++)
        {
            var ci = new DItem[rnd.Next(20, 50)];
            for (int j = 0; j < ci.Length; j++)
                ci[j] = new DItem(new Uri($"https://source.net/file/{i + j}.jpg"), $"C:\\{userName}\\{i + j}");
            cc[i] = new DItemsCollection(Faker.Lorem.Sentence(2), ci);
        }

        using SemaphoreSlim ss = new SemaphoreSlim(2);
        using CancellationTokenSource cts = new CancellationTokenSource();
        var tasks = cc.Select(x => x.Download(ss, 2, cts.Token)).ToArray();

        Console.CancelKeyPress += (s, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
            LoggerFactory.Logger.Warn("Cancellation requested");
        };

        Task.WaitAll(tasks);
    }
}
