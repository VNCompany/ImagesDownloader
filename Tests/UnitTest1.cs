using System.IO;

using HtmlParsing.Internal;

namespace Tests
{
    public class Tests
    {
        private void PrintTag(HtmlReader reader, TagInfo tagInfo)
        {
            Console.Write($"{reader.GetString(tagInfo.NameRange)} {reader.GetString(tagInfo.Range)}");
            if (tagInfo.IsCloseTag)
                Console.WriteLine(" - close tag");
            else
                Console.WriteLine();
        }

        [Test]
        public void Test1()
        {
            HtmlReader reader = new HtmlReader(File.ReadAllText(@"E:\Projects\ImagesDownloader\Tests\test.small2.html"));
        
            while (reader.ReadTag(out TagInfo tagInfo))
                PrintTag(reader, tagInfo);
        }
    }
}