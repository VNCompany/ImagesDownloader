using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

using HtmlParsing.Internal;

namespace HtmlParsing
{
    public class HtmlNode
    {
        private readonly HtmlParser _parser;
        private Dictionary<string, StringRange>? _attrs;
        internal NodeSchema Schema;

        private void LoadAttributes()
        {
            _attrs = new Dictionary<string, StringRange>(_parser.GetNodeAttributes(this));
        }

        internal HtmlNode(HtmlParser parser, string name, NodeSchema schema)
        {
            _parser = parser;
            Name = name;
            Schema = schema;
        }

        public string Name { get; }
        
        public string? Id { get; internal set; }

        public HtmlValue? Content { get; internal set; }
        
        public HtmlNode? Parent { get; internal set; }
        
        public HtmlNodesCollection? Childs { get; internal set; }

        public ICollection<string> Attributes
        {
            get
            {
                if (_attrs == null)
                    LoadAttributes();

                return _attrs!.Keys;
            }
        }

        public bool TryGetAttribute(string name, [NotNullWhen(true)] out HtmlValue? value)
        {
            if (_attrs == null)
                LoadAttributes();

            if (_attrs!.TryGetValue(name, out StringRange valueRange))
            {
                value = _parser.BuildHtmlValue(valueRange);
                return true;
            }

            value = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HtmlValue? GetAttribute(string name)
            => TryGetAttribute(name, out HtmlValue? result) ? result : null;

        public bool HasClasses(IEnumerable<string> names)
        {
            if (TryGetAttribute("class", out HtmlValue? classAttr))
            {
                string[] nodeClasses = StringTools.SpaceSplit(classAttr.Value).ToArray();
                return names.All(className => nodeClasses.Contains(className));
            }
            
            return false;
        }

        public bool HasClass(string name) => HasClasses(new[] { name });
    }
}

