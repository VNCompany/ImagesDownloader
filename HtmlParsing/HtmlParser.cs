using System.Collections.Generic;

using HtmlParsing.Internal;

namespace HtmlParsing
{
    public class HtmlParser
    {
        private readonly HtmlReader _reader;
        private readonly Dictionary<string, HtmlNode> _ids;
        private readonly List<HtmlNode> _nodes;

        public string Content => _reader.Content;
        
        public HtmlNode Head { get; }
        public HtmlNode Body { get; }
        
        public HtmlParser(string html, bool loadAttributes = true)
        {
            _reader = new HtmlReader(html);
            _nodes = new List<HtmlNode>();
            _ids = loadAttributes ? new Dictionary<string, HtmlNode>() : new Dictionary<string, HtmlNode>(0);
            
            while (_reader.ReadTag(out StringRange tagRange))
            {
                bool isCloseTag = _reader.Content[tagRange.Start + 1] == '/';
                StringRange nameRange = _reader.GetTagName(tagRange, isCloseTag);
                
                if (nameRange.Length == 0) continue;
                
                string tagName = _reader.GetString(nameRange).ToLower();
                
                if (isCloseTag == false)
                {
                    NodeSchema nodeSchema = new NodeSchema()
                    {
                        Index = _nodes.Count,
                        Range = tagRange
                    };
                    HtmlNode htmlNode = new HtmlNode(this, tagName, nodeSchema);
                    _nodes.Add(htmlNode);
                    if (Head == null && tagName == "head") Head = htmlNode;
                    else if (Body == null && tagName == "body") Body = htmlNode;
                }
                else
                {
                    // Получение корневого узла
                    int startNodeIndex = _nodes.FindLastIndex(node => node.Name == tagName);
                    if (startNodeIndex != -1 && _nodes.Count - startNodeIndex > 1)
                    {
                        // Если найден корневой узел, и есть дочерние узлы, то указываем
                        // родителю последний дочерний индекс и диапазон строки с телом узла
                        HtmlNode node = _nodes[startNodeIndex];
                        node.Schema.LastIndex = _nodes.Count - 1;
                        node.Content = new HtmlValue(_reader.Content, tagRange - node.Schema.Range);
                        
                        // Всем дочерним элементам узла указываем их родителя
                        for (int i = startNodeIndex + 1; i != _nodes.Count; i++)
                            if (_nodes[i].Parent == null)
                                _nodes[i].Parent = node;
                    }
                }
            }
        }
    }
}

