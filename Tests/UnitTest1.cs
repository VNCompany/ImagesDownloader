using HtmlParsing.Internal;
using HtmlParsing;

namespace Tests
{
    public class Tests
    {
        private void PrintNode(HtmlNode? node)
        {
            Console.WriteLine("----------------");
            if (node is null)
            {
                Console.WriteLine("NULL");
                return;
            }

            Console.WriteLine("Name: {0}", node.Name);
            Console.WriteLine("Content size: {0}", node.Content?.Length ?? 0);
            Console.WriteLine("Parent node name: {0}", node.Parent?.Name ?? "NULL");
            Console.WriteLine("Childs count: {0}", node.Childs?.Count ?? 0);
            Console.WriteLine("Attributes: {0}", string.Join(", ", node.Attributes));
            
            Console.WriteLine("----------------");
            Console.WriteLine();
        }
        
        [Test]
        public void Test1()
        {
            var reader = new HtmlParser(File.ReadAllText(@"E:\Projects\ImagesDownloader\Tests\test.small2.html"));

            var node = reader.GetElementById("test-id");
            PrintNode(node);
            Console.WriteLine(node?.Content?.Value);
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