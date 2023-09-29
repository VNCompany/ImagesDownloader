using HtmlParsing.Internal;
using HtmlParsing;
using XPathParsing;

namespace Tests
{
    public class Tests
    {
        private void PrintNode(HtmlNode? node, string prefix = "")
        {
            if (node is null)
            {
                Console.WriteLine("<NULL>");
                return;
            }
            
            Console.WriteLine("{4}<{0} AttributesCount='{1}' ChildsCount='{2}' ContentSize='{3}'>",
                node.Name,
                node.Attributes.Count,
                node.Childs?.Count ?? 0,
                node.Content?.Length,
                prefix);
        }
        
        [Test]
        public void Test1()
        {
            var reader = new HtmlParser(File.ReadAllText(@"E:\Projects\ImagesDownloader\Tests\test.html"));

            var query = from lnk in
                    (from container in reader.Body.GetByClassNames(new[] { "image-container", "column" })
                        select container.GetByClassName("image").FirstOrDefault()?.GetAttribute("href"))
                where lnk != null
                select lnk;
            
            foreach (var imgLink in query.Take(10))
                Console.WriteLine(imgLink);
        }

        [Test]
        public void TagAttributesParseTest()
        {
            HtmlReader r = new HtmlReader("<p   a=12  readonly value=\"hello world\" disabled>");
            var tagAttrs = r.ReadTagAttributes(new StringRange(0, 49), 1);
            Assert.That(tagAttrs, Has.Count.GreaterThanOrEqualTo(4));
            Assert.Multiple(() =>
            {
                Assert.That(r.GetString(tagAttrs[0].Key), Is.EqualTo("a"), "tagAttrs[0]");
                Assert.That(r.GetString(tagAttrs[0].Value), Is.EqualTo("12"), "tagAttrs[0]");
                
                Assert.That(r.GetString(tagAttrs[1].Key), Is.EqualTo("readonly"), "tagAttrs[1]");
                Assert.That(r.GetString(tagAttrs[1].Value), Is.EqualTo("readonly"), "tagAttrs[1]");
                
                Assert.That(r.GetString(tagAttrs[2].Key), Is.EqualTo("value"), "tagAttrs[2]");
                Assert.That(r.GetString(tagAttrs[2].Value), Is.EqualTo("hello world"), "tagAttrs[2]");
                
                Assert.That(r.GetString(tagAttrs[3].Key), Is.EqualTo("disabled"), "tagAttrs[3]");
                Assert.That(r.GetString(tagAttrs[3].Value), Is.EqualTo("disabled"), "tagAttrs[3]");
            });
        }
    }
}