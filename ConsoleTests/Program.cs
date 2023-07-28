using System.Diagnostics;
using HtmlParsing.Internal;

namespace ConsoleTests;

class Program
{
    static void PrintTag(HtmlReader reader, TagInfo tagInfo)
    {
        Console.Write($"{reader.GetString(tagInfo.NameRange)} {reader.GetString(tagInfo.Range)}");
        if (tagInfo.IsCloseTag)
            Console.WriteLine(" - close tag");
        else
            Console.WriteLine();
    }
    
    static void Main(string[] args)
    {
        HtmlReader reader = new HtmlReader(File.ReadAllText(@"E:\Projects\ImagesDownloader\Tests\test.small2.html"));

        int counter = 0;
        while (reader.ReadTag(out TagInfo tagInfo))
        {
            PrintTag(reader, tagInfo);
            counter++;
        }

        Console.WriteLine();
        Console.WriteLine("Count: {0}", counter);
        Console.WriteLine();

        Stopwatch sw = Stopwatch.StartNew();
        StringRange closeTag = reader.FindCloseTag("script", 0);
        if (closeTag.Length > 0)
            Console.WriteLine(reader.GetString(closeTag));
        sw.Stop();

        Console.WriteLine(sw.Elapsed);
    }
}