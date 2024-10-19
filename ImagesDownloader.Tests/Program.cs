﻿using ImagesDownloader.Core.Factories;
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

        var dman = new DManager(2, 2);
        dman.Start(cc);

        Console.CancelKeyPress += (s, e) =>
        {
            dman.Stop();
        };

        while (dman.IsRunning)
            Thread.Sleep(100);

        dman.Stop();
    }
}
