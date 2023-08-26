using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using XPathParsing;

namespace HtmlParsing.XPathSelector
{
    public class XPathConveyer
    {
        private bool _relative;

        public IEnumerable Selection { get; private set; }

        public XPathConveyer(HtmlNode root)
        {
            Selection = new[] { root };
        }

        public void Append(XPathNode xPathNode)
        {
            if (xPathNode.Element == null)
            {
                if (!_relative)
                {
                    _relative = true;
                    return;
                }
                else ThrowHelper.ThrowInvalidXPathSequence();
            }

            if (Selection is IEnumerable<HtmlNode> tmp)
            {
                if (xPathNode.Element!.ValueType == XPathValueType.Attribute)
                {
                    Selection = tmp.Select(node => node.GetAttribute(xPathNode.Element!.Value)?.Value);
                    return;
                }
                
                if (_relative)
                {
                    _relative = false;
                    tmp = tmp.SelectMany(node => node.GetAllElements());
                }
                else
                    tmp = tmp.SelectMany(node => node.Childs ?? Enumerable.Empty<HtmlNode>());

                tmp = xPathNode.Element!.Value switch
                {
                    ".." => (IEnumerable<HtmlNode>)Selection,
                    "*" => tmp,
                    _ => tmp.Where(node => node.Name == xPathNode.Element!.Value)
                };

                if (xPathNode.Filter is XPathIndexFilter xPathIndexFilter)
                    tmp = tmp.Skip(xPathIndexFilter.Index).Take(1);
                else if (xPathNode.Filter is XPathFilter xPathFilter)
                {
                    if (xPathFilter.Name.ValueType == XPathValueType.Element)
                        tmp = tmp.Where(node => node.Childs != null
                                                && node.Childs.Any(cn => cn.Name == xPathFilter.Name.Value
                                                                         && cn.Content != null
                                                                         && xPathFilter.Validate(cn.Content.Value)));
                    else
                        tmp = tmp.Where(node =>
                            node.TryGetAttribute(xPathFilter.Name.Value, out HtmlValue? nodeAttr)
                            && xPathFilter.Validate(nodeAttr.Value));
                }

                Selection = tmp;
            }
            else ThrowHelper.ThrowInvalidXPathSequence();
        }

        public IEnumerable<string?> GetStrings()
        {
            return Selection switch
            {
                IEnumerable<string?> values => values,
                IEnumerable<HtmlNode> nodes => nodes.Select(node => node.Content?.Value),
                _ => Enumerable.Empty<string?>()
            };
        }

        public IEnumerable<HtmlNode> GetNodes()
            => Selection as IEnumerable<HtmlNode> ??
               throw new InvalidOperationException("Invalid collection type: Selection isn't an HtmlNode collection");
    }
}

