using HtmlParsing;
using System.Diagnostics;

namespace Tests;

public class Tests
{
    [Test]
    public void Test1()
    {
        string html = File.ReadAllText("test.html");
        
        Stopwatch sw = Stopwatch.StartNew();
        HtmlParser hp = new HtmlParser(html);
        var attrs = hp.Tags.Select(t => t.Attributes.ToArray()).ToArray();
        sw.Stop();

        Console.WriteLine("Time: {0} ms", sw.ElapsedMilliseconds);
        Console.WriteLine("Tags: {0} pcs", hp.Tags.Count);
        Console.WriteLine("Img attributes: {0} pcs", attrs.Length);
    }
}