using System.Linq;
using System.Collections.Generic;
using System.Collections;

using XPathParsing;

namespace HtmlParsing.XPathSelector
{
    public static class HtmlParserExtensions
    {
        private static XPathConveyer? SelectPathConveyer(HtmlParser htmlParser, XPathParser xPathParser)
        {
            if (xPathParser.Nodes.Count == 0) return null;

            XPathValue? rootPathValue = xPathParser.Nodes[0].Element;
            if (rootPathValue == null
                || rootPathValue.ValueType == XPathValueType.Attribute)
                throw ThrowHelper.InvalidXPathSequence("the root element cannot be an attribute");

            HtmlNode root = rootPathValue.Value switch
            {
                "body" => htmlParser.Body,
                "head" => htmlParser.Head,
                _ => throw ThrowHelper.InvalidXPathSequence("invalid root element (allowed head and body)")
            };

            XPathConveyer conveyer = new XPathConveyer(root);
            for (int i = 1; i != xPathParser.Nodes.Count; i++)
                conveyer.Append(xPathParser.Nodes[i]);
            
            return conveyer;
        }

        private static XPathConveyer? SelectPathConveyer(HtmlParser htmlParser, string xPathPattern)
            => SelectPathConveyer(htmlParser, new XPathParser(xPathPattern));

        public static IEnumerable Select(this HtmlParser htmlParser, string xPathPattern)
            => SelectPathConveyer(htmlParser, xPathPattern)?.Selection ?? Enumerable.Empty<object>();

        public static IEnumerable<HtmlNode> SelectNodes(this HtmlParser htmlParser, string xPathPattern)
            => SelectPathConveyer(htmlParser, xPathPattern)?.GetNodes() ?? Enumerable.Empty<HtmlNode>();

        public static IEnumerable<string?> SelectStrings(this HtmlParser htmlParser, XPathParser xPathParser)
            => SelectPathConveyer(htmlParser, xPathParser)?.GetStrings() ?? Enumerable.Empty<string?>();

        public static IEnumerable<string?> SelectStrings(this HtmlParser htmlParser, string xPathPattern)
            => SelectPathConveyer(htmlParser, xPathPattern)?.GetStrings() ?? Enumerable.Empty<string?>();
    }
}