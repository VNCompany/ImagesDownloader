using XPathParsing;
using XPathParsing.Internal;
using PToken = XPathParsing.Internal.XPathElementParser.Token;
using PTokenType = XPathParsing.Internal.XPathElementParser.TokenType;

namespace Tests.XPathParsingTests
{
    [TestFixture]
    public class XPathNodeParserTests
    {
        private void XPathNodeParser_Validate(string pattern, IList<PToken> values)
        {
            var parsed = XPathElementParser.GetNodeTokens(pattern);
            Assert.Multiple(() =>
            {
                Assert.That(parsed, Has.Count.EqualTo(values.Count));

                for (int i = 0; i != values.Count; ++i)
                {
                    Assert.That(parsed[i].Type, Is.EqualTo(values[i].Type));
                    Assert.That(parsed[i].Value, Is.EqualTo(values[i].Value));
                }
            });
        }

        [TestCase("@")]
        [TestCase("[v='hello]")]
        [TestCase("[v=%'hello']")]
        [TestCase("['hello'='world']")]
        [TestCase("[&amp;='value']")]
        [TestCase("@='value'")]
        [TestCase("==")]
        [TestCase("'first''second'")]
        public void XPathNodeParser_GetNodeTokens_Exceptions(string pattern)
        {
            Assert.Throws<InvalidXPathException>(() =>
            {
                XPathElementParser.GetNodeTokens(pattern);
            });
        }
        
        [Test]
        public void XPathNodeParser_GetNodeTokens()
        {
            var testCases = new[]
            {
                ("div", new[]
                {
                    new PToken(PTokenType.Id, "div")
                }),
                ("[@data-test='hello world']", new[]
                {
                    new PToken(PTokenType.Block, "["),
                    new PToken(PTokenType.Id, "@data-test"),
                    new PToken(PTokenType.Operator, "="),
                    new PToken(PTokenType.Literal, "hello world"),
                    new PToken(PTokenType.Block, "]")
                }),
                ("test_element [@data-test %= 'hello world']", new[]
                {
                    new PToken(PTokenType.Id, "test_element"),
                    new PToken(PTokenType.Block, "["),
                    new PToken(PTokenType.Id, "@data-test"),
                    new PToken(PTokenType.Operator, "%="),
                    new PToken(PTokenType.Literal, "hello world"),
                    new PToken(PTokenType.Block, "]")
                }),
                ("[v$=i\"value\"]", new[]
                {
                    new PToken(PTokenType.Block, "["),
                    new PToken(PTokenType.Id, "v"),
                    new PToken(PTokenType.Operator, "$="),
                    new PToken(PTokenType.Id, "i"),
                    new PToken(PTokenType.Literal, "value"),
                    new PToken(PTokenType.Block, "]")
                }),
            };
            
            foreach (var testCase in testCases)
                XPathNodeParser_Validate(testCase.Item1, testCase.Item2);
        }
    }
}