using System.Linq;

using XPathParsing;

namespace Tests.XPathParsingTests
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

        private XPathElement GetXPathElement(string pattern)
        {
            var ts = XPathParsing.Internal.XPathElementParser.GetNodeTokens(pattern).ToArray();
            return new XPathElement(ts, false);
        }
        
        [Test]
        public void XPathConditionIndex()
        {
            Assert.Throws<InvalidXPathException>(() =>
            {
                GetXPathElement("[]");
            });
            
            Assert.Throws<InvalidXPathException>(() =>
            {
                GetXPathElement("[NaN]");
            });

            var elem = GetXPathElement("[2]");
            var condition = elem.Condition ?? throw new Exception();
            Assert.That(condition.Apply(testData).Single().Value, Is.EqualTo("system"));
        }

        [Test]
        public void XPathConditionFilter_Simple()
        {
            var condition = GetXPathElement("[default%=i'w']").Condition!;
            Assert.That(
                condition.Apply(testData.Select(td => td.Value)).ToArray(), 
                Is.EquivalentTo(new[] { "wrapper", "two", "Hello World!" }));
            
            condition = GetXPathElement("[default%='w']").Condition!;
            Assert.That(
                condition.Apply(testData.Select(td => td.Value)).ToArray(),
                Is.EquivalentTo(new[] { "wrapper", "two"}));
        }

        class XPathProvider : IXPathProvider
        {
            private readonly KeyValuePair<string, string> _item;
            
            private XPathProvider(KeyValuePair<string, string> item) => _item = item;

            public bool ProcessCondition(XPathConditionFilter filter) 
                => filter.ElementName.Value == _item.Key && filter.Predicate(_item.Value);

            public static XPathProvider GetProvider(KeyValuePair<string, string> item) => new XPathProvider(item);
        }

        [Test]
        public void XPathConditionFilter_Composite()
        {
            var condition = GetXPathElement("[class^=i'WRAP']").Condition!;
            var collectionResult = condition.Apply(testData, XPathProvider.GetProvider).ToArray();
            Assert.That(collectionResult, Has.Length.EqualTo(1));
            Assert.Multiple(() =>
            {
                Assert.That(collectionResult[0].Key, Is.EqualTo("class"));
                Assert.That(collectionResult[0].Value, Is.EqualTo("wrapper"));
            });
        }
    }
}

