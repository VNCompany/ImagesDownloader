using System;
using System.IO;

using HtmlParsing.Internal;

string html = File.ReadAllText("test.small2.html");

HtmlReader htmlReader = new HtmlReader(html);

int counter = 1;
TagResult tagInfo;
while ((tagInfo = htmlReader.GetTag()).Success)
{
    if (tagInfo.Range.Length >= 3)
    {
        bool isCloseTag = htmlReader[tagInfo.Range.Start + 1] == '/';
        string name = htmlReader.ToString(htmlReader.GetTagName(tagInfo.Range, isCloseTag));
        Console.WriteLine("{0}. {1} {2}: {3}",
            counter++,
            isCloseTag ? "Close tag" : "Start tag",
            name,
            htmlReader.ToString(tagInfo.Range));
    }
}