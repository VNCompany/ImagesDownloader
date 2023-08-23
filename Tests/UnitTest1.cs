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

        private void PrintAllNodes(IReadOnlyList<HtmlNode> allNodes, HtmlNode currentNode, int tab = 0)
        {
            PrintNode(currentNode, new string('\t', tab));

            foreach (var htmlNode in allNodes.Where(n => n.Parent == currentNode))
                PrintAllNodes(allNodes, htmlNode, tab + 1);
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

        [TestCase("div='hello world'", false)]
        [TestCase("@div='hello world'", false)]
        [TestCase("@div=i'hello world'", false)]
        [TestCase("@div%=i'hello world'", false)]
        [TestCase("@div%=i'h'", false)]
        [TestCase("@div%=i''", false)]
        [TestCase("v=i''", false)]
        [TestCase("v=''", false)]
        [TestCase("@div%=ihello world'", true)]
        [TestCase("@div%=i'hello world", true)]
        [TestCase("@div%=hello world", true)]
        [TestCase("@div=hello world", true)]
        [TestCase("div=hello world", true)]
        [TestCase("=hello world", true)]
        [TestCase("hello world", true)]
        [TestCase("hello", true)]
        [TestCase("='hello'", true)]
        [TestCase("%='hello'", true)]
        [TestCase("%=i", true)]
        [TestCase("%='i'", true)]
        public void XPathValidationFilterParser(string pattern, bool isNull)
        {
            if ((XPathFilter.Parse(pattern) == null) != isNull)
                Assert.Fail();
        }

        [TestCase("div='hello world'", "div", "hello world", false, XPathSearchOption.Equality)]
        [TestCase("@div='hello world'", "@div", "hello world", false, XPathSearchOption.Equality)]
        [TestCase("@div%='hello world'", "@div", "hello world", false, XPathSearchOption.Contains)]
        [TestCase("@div%=i'hello world'", "@div", "hello world", true, XPathSearchOption.Contains)]
        [TestCase("@div^=i'hello world'", "@div", "hello world", true, XPathSearchOption.StartsWith)]
        [TestCase("@div$=i'hello world'", "@div", "hello world", true, XPathSearchOption.EndsWith)]
        [TestCase("div$=i'hello world'", "div", "hello world", true, XPathSearchOption.EndsWith)]
        [TestCase("div$=i'h'", "div", "h", true, XPathSearchOption.EndsWith)]
        [TestCase("div='h'", "div", "h", false, XPathSearchOption.Equality)]
        public void XPathCheckFilterParser(string pattern, string name, string value, bool caseInsensitive, 
            XPathSearchOption option)
        {
            XPathFilter? filter = XPathFilter.Parse(pattern);
            Assert.That(filter, Is.Not.Null);
            
            if (filter == null) return;
            
            Assert.Multiple(() =>
            {
                Assert.That(filter.Name.ValueType == XPathValueType.Attribute
                    ? "@" + filter.Name.Value
                    : filter.Name.Value, Is.EqualTo(name));
                
                Assert.That(filter.Value, Is.EqualTo(value));
                
                Assert.That(filter.CaseInsensitive, Is.EqualTo(caseInsensitive));
                
                Assert.That(filter.Option, Is.EqualTo(option));
            });
        }

        [Test]
        public void XPathTest()
        {
            // string pattern = "//main/div[3]/p[@class%='test-class']/img[test=i'value']/@src";
            string pattern = "div[@class^=i'cls']/@src";
            XPathParser parser = new XPathParser(pattern);
            Console.WriteLine("Pattern: {0}", pattern);
            Console.WriteLine("Parsed: {0}", parser);
        }
    }
}