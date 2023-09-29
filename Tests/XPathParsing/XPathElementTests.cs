using System.Linq;

using XPathParsing;
using Parser = XPathParsing.Internal.XPathElementParser;

namespace Tests.XPathParsing
{
    [TestFixture]
    public class XPathElementTests
    {
        private readonly Dictionary<string, string> testData = new Dictionary<string, string>
        {
            ["key"] = "value",
            ["class"] = "wrapper",
            ["data-object"] = "system",
            ["w-one"] = "two",
            ["value"] = "Hello World!"
        };
        
        [Test]
        public void XPathConditionIndex()
        {
            Assert.Throws<InvalidXPathException>(() =>
            {
                var ts = Parser.GetNodeTokens("[]").ToArray();
                var e = new XPathElement(ts);
            });
            
            Assert.Throws<InvalidXPathException>(() =>
            {
                var ts = Parser.GetNodeTokens("[NaN]").ToArray();
                var e = new XPathElement(ts);
            });

            var tokens = Parser.GetNodeTokens("[2]").ToArray();
            var e = new XPathElement(tokens);
            
            Assert.That(e.Condition, Is.Not.Null);
            Assert.Multiple(() =>
            {
                // Assert.That(e.Condition!.Apply(new XPathConditionArgs(testData)).SingleOrDefault().Key, Is.EqualTo("data-object"));
                // Assert.That(e.Condition!.Apply(testData).SingleOrDefault().Value, Is.EqualTo("system"));
            });
        }

        [Test]
        public void XPathConditionFilter()
        {
            var tokens = Parser.GetNodeTokens("[class='wrapper']");
        }
    }
}

