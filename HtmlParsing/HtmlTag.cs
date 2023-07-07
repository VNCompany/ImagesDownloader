using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace HtmlParsing
{
    public class HtmlTag
    {
        private Dictionary<string, string>? attrs;

        public TagSchema Schema { get; }
        public string Name { get; }
        
        public HtmlTag? Parent { get; internal set; }
        public HtmlContent? Content { get; internal set; }

        public HtmlTag(TagSchema schema, string name)
        {
            Schema = schema;
            Name = name;
        }

        private Dictionary<string, string> _Attributes
        {
            get
            {
                if (attrs == null)
                    attrs = new Dictionary<string, string>(
                        ParserExtensions.ParseTagAttributes(Schema.GetBodySpan().Slice(Name.Length)));
                return attrs;
            }
        }

        public string? GetAttribute(string name)
        {
            if (_Attributes.TryGetValue(name, out var value))
                return value;
            return null;
        }

        public IEnumerable<KeyValuePair<string, string>> Attributes => _Attributes;
    }
}