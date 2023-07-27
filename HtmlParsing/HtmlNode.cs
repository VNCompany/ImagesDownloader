using HtmlParsing.Internal;

namespace HtmlParsing
{
    public class HtmlNode
    {
        private readonly HtmlParser _parser;
        
        internal NodeSchema Schema;
        
        public string Name { get; }
        
        public HtmlValue Content { get; internal set; }
        
        public HtmlNode Parent { get; internal set; }

        internal HtmlNode(HtmlParser parser, string name, NodeSchema schema)
        {
            _parser = parser;
            Name = name;
            Schema = schema;
        }
    }
}

