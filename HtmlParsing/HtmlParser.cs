using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using HtmlParsing.Internal;

namespace HtmlParsing
{
    public class HtmlParser
    {
        private readonly string _html;
        private readonly HtmlReader _reader;
        private readonly Dictionary<string, HtmlNode>? _ids;
        private readonly List<HtmlNode> _nodes;

        internal IEnumerable<KeyValuePair<string, StringRange>> GetNodeAttributes(HtmlNode node)
            => _reader.ReadTagAttributes(node.Schema.Range, node.Name.Length)
                .Select(attr =>
                    new KeyValuePair<string, StringRange>(_reader.GetString(attr.Key).ToLower(), attr.Value));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal HtmlValue BuildHtmlValue(StringRange range)
            => new HtmlValue(_html, range);

        public IReadOnlyList<HtmlNode> Nodes => _nodes;

        public HtmlNode Head { get; } = null!;
        public HtmlNode Body { get; } = null!;
        
        /// <summary>
        /// lxml parser for default HTML and HTML5
        /// </summary>
        /// <param name="html">HTML document</param>
        /// <param name="lazyLoad">if true, the attributes of the elements will be loaded dynamically. Method GetElementById isn't available</param>
        /// <exception cref="ArgumentNullException">argument html null or empty</exception>
        public HtmlParser(string html, bool lazyLoad = false)
        {
            if (string.IsNullOrEmpty(html))
                throw new ArgumentNullException(nameof(html));
            
            _html = html;
            _reader = new HtmlReader(html);
            _nodes = new List<HtmlNode>();
            _ids = !lazyLoad ? new Dictionary<string, HtmlNode>() : null;
            
            while (_reader.ReadTag(out TagInfo tagInfo))
            {
                string tagName = _reader.GetString(tagInfo.NameRange).ToLower();
                
                if (tagInfo.IsCloseTag == false)
                {
                    NodeSchema nodeSchema = new NodeSchema()
                    {
                        Index = _nodes.Count,
                        Range = tagInfo.Range
                    };
                    HtmlNode htmlNode = new HtmlNode(this, tagName, nodeSchema);
                    _nodes.Add(htmlNode);
                    
                    if (Head == null && tagName == "head") Head = htmlNode;
                    else if (Body == null && tagName == "body") Body = htmlNode;

                    if (_ids != null)
                    {
                        if (htmlNode.TryGetAttribute("id", out HtmlValue? idAttr))
                        {
                            htmlNode.Id = idAttr.Value;
                            _ids[htmlNode.Id] = htmlNode;
                        }
                    }
                }
                else
                {
                    // Получение корневого узла
                    int startNodeIndex = _nodes.FindLastIndex(node => node.Name == tagName 
                                                                      && node.Schema.LastIndex == -1);
                    
                    if (startNodeIndex != -1)
                    {
                        // Если найден корневой узел, и есть дочерние узлы, то указываем
                        // родителю последний дочерний индекс и диапазон строки с телом узла
                        HtmlNode node = _nodes[startNodeIndex];
                        node.Schema.LastIndex = _nodes.Count - 1;
                        node.Content = new HtmlValue(html, tagInfo.Range - node.Schema.Range);
                        
                        // Есть ли дочерние элементы
                        if (_nodes.Count - startNodeIndex > 1)
                        {
                            startNodeIndex += 1;
                    
                            // Всем дочерним элементам узла указываем их родителя
                            for (int i = startNodeIndex; i != _nodes.Count; i++)
                                if (_nodes[i].Parent == null)
                                    _nodes[i].Parent = node;
                    
                            // Задать коллекцию дочерних элементов элементу
                            node.Childs = new HtmlNodesCollection(_nodes, node, startNodeIndex,
                                node.Schema.LastIndex);
                        }
                    }
                }
            }
        }

        
        /// <summary>
        /// Gets element by id
        /// </summary>
        /// <param name="name">element id</param>
        /// <returns>object of node, null if not found</returns>
        /// <exception cref="NotSupportedException">enabled lazy loading mode</exception>
        public HtmlNode? GetElementById(string name)
        {
            if (_ids == null)
                throw new NotSupportedException("Enabled lazy loading mode");
            
            if (_ids.TryGetValue(name, out HtmlNode? node))
                return node;
            return null;
        }
    }
}

