namespace XPathParsing
{
    public class XPathIndexFilter : IXPathFilter
    {
        public int Index { get; }

        public XPathIndexFilter(int index)
        {
            Index = index;
        }

        public override string ToString() => $"Index({Index})";
    }
}