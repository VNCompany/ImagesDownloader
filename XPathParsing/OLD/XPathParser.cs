using System;
using System.Linq;
using System.Collections.Generic;

namespace OLD.XPathParsing
{
    public class XPathParser
    {
        private readonly XPathNode[] _nodes;

        public IReadOnlyList<XPathNode> Nodes => _nodes;
        
        public XPathParser(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern)) ThrowHelper.ThrowInvalidPatternException();

            string[] stringNodes = pattern.Split('/').Select(ns => ns.Trim()).ToArray();
            _nodes = new XPathNode[stringNodes.Length];

            for (int i = 0; i != stringNodes.Length; i++)
            {
                string stringNode = stringNodes[i];
                if (string.IsNullOrEmpty(stringNode))
                {
                    _nodes[i] = new XPathNode(null, null);
                    continue;
                }
                
                if (stringNode[0] == '[') ThrowHelper.ThrowInvalidNodeException(i, stringNode);
                
                int filterIndex = stringNode.IndexOf('[');

                XPathValue? nodeElement = XPathValue.Parse(filterIndex == -1
                    ? stringNode
                    : stringNode.Substring(0, filterIndex));
                
                if (nodeElement == null) ThrowHelper.ThrowInvalidNodeValueException(i, stringNode);

                IXPathFilter? nodeFilter = null;
                if (filterIndex != -1)
                {
                    if (stringNode[^1] != ']') ThrowHelper.ThrowInvalidNodeException(i, stringNode);

                    ReadOnlySpan<char> filterSpan = stringNode.AsSpan(filterIndex + 1, 
                        stringNode.Length - filterIndex - 2).Trim();
                    
                    if (filterSpan.Length == 0) ThrowHelper.ThrowInvalidNodeFilterException(i, stringNode);

                    if (!filterSpan.Contains('='))
                    {
                        if (int.TryParse(filterSpan, out int filterValue))
                            nodeFilter = new XPathIndexFilter(filterValue);
                        else
                            ThrowHelper.ThrowInvalidNodeFilterException(i, stringNode);
                    }
                    else if ((nodeFilter = XPathFilter.Parse(filterSpan)) == null)
                        ThrowHelper.ThrowInvalidNodeFilterException(i, stringNode);
                }

                _nodes[i] = new XPathNode(nodeElement, nodeFilter);
            }
        }

        public override string ToString() => string.Join("/", from node in _nodes select node.ToString());
    }
}

