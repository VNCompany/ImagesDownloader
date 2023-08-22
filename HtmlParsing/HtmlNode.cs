using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
            Schema.LastIndex = -1;
        }

        public string Name { get; }
        
        public string? Id { get; internal set; }

        public HtmlValue? Content { get; internal set; }
        
        public HtmlNode? Parent { get; internal set; }
        
        public HtmlNodesCollection? Childs { get; internal set; }
        
        /// <summary>
        /// Collection of attributes keys
        /// </summary>
        public ICollection<string> Attributes
        {
            get
            {
                if (_attrs == null)
                    LoadAttributes();

                return _attrs!.Keys;
            }
        }

        
        /// <summary>
        /// Try get element attribute
        /// </summary>
        /// <param name="name">attribute name</param>
        /// <param name="value">out attribute value; null, if not found</param>
        /// <returns>true, if the attribute is found</returns>
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

        /// <summary>
        /// Get element attribute
        /// </summary>
        /// <param name="name">attribute name</param>
        /// <returns>attribute value or null if not found</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HtmlValue? GetAttribute(string name)
            => TryGetAttribute(name, out HtmlValue? result) ? result : null;

        /// <summary>
        /// Check for all element classes availability
        /// </summary>
        /// <param name="names">names of classes</param>
        /// <returns>true, if all the 'names' enumeration classes are present in the element</returns>
        public bool HasClasses(IEnumerable<string> names)
        {
            if (TryGetAttribute("class", out HtmlValue? classAttr))
            {
                string[] nodeClasses = StringTools.SpaceSplit(classAttr.Value).ToArray();
                return names.All(className => nodeClasses.Contains(className));
            }
            
            return false;
        }

        /// <summary>
        /// Check for class availability in element
        /// </summary>
        /// <param name="name">class name</param>
        /// <returns>true if found</returns>
        public bool HasClass(string name) => HasClasses(new[] { name });

        /// <summary>
        /// Get elements by class names
        /// </summary>
        /// <param name="names">class names</param>
        /// <returns>collection of html nodes</returns>
        public IEnumerable<HtmlNode> GetByClassNames(IEnumerable<string> names)
        {
            IList<string> classNames = names as IList<string> ?? names.ToArray();
            if (Childs?.Count > 0 && classNames.Count > 0)
            {
                for (int i = Schema.Index + 1; i <= Schema.LastIndex; i++)
                {
                    HtmlNode node = _parser.Nodes[i];
                    if (node.HasClasses(classNames))
                        yield return node;
                }
            }
        }

        /// <summary>
        /// Get elements by class name
        /// </summary>
        /// <param name="name">class name</param>
        /// <returns>collection of html nodes</returns>
        public IEnumerable<HtmlNode> GetByClassName(string name) => GetByClassNames(new[] { name });

        /// <summary>
        /// Search through all nested elements
        /// </summary>
        /// <param name="predicate">predicate</param>
        /// <returns>collection of html nodes</returns>
        public IEnumerable<HtmlNode> Find(Predicate<HtmlNode> predicate)
        {
            if (Childs?.Count > 0)
                for (int i = Schema.Index + 1; i <= Schema.LastIndex; i++)
                    if (predicate.Invoke(_parser.Nodes[i]))
                        yield return _parser.Nodes[i];
        }
    }
}

