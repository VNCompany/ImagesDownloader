

namespace XPathParsing
{
    public class XPathNode
    {
        public XPathValue? Element { get; }
        public IXPathFilter? Filter { get; }

        public XPathNode(XPathValue? element, IXPathFilter? filter)
        {
            Element = element;
            Filter = filter;
        }

        public override string ToString()
            => Element == null
                ? "EmptyNode()"
                : (Filter == null ? $"XPathNode({Element})" : $"XPathNode({Element}, {Filter})");
    }
}