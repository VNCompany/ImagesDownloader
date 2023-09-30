using System;
using System.Linq;
using System.Collections.Generic;

using Parser = XPathParsing.Internal.XPathElementParser;

namespace XPathParsing
{
    public class XPathElement
    {
        public XPathName? ElementName { get; }
        
        public XPathConditionBase? Condition { get; }
        
        public bool IsRelative { get; }
        
        internal XPathElement(ReadOnlySpan<Parser.Token> tokens, bool isRelative)
        {
            IsRelative = isRelative;
            
            int ptr = 0;
            if (tokens[ptr].Type == Parser.TokenType.Id)
                ElementName = new XPathName(tokens[ptr++].Value);

            if (tokens.Length != 1)
            {
                int conditionTokensCount = tokens.Length - ptr;
                if (conditionTokensCount is < 3 or > 6
                    || tokens[ptr].Value != "["
                    || tokens[^1].Value != "]")
                    throw new InvalidXPathException(string.Concat(tokens.ToArray().Select(t => t.Value)),
                        "Invalid XPathElement structure");
                
                Condition = XPathConditionBase.Create(tokens.Slice(++ptr, tokens.Length - ptr - 1));
            }
        }
    }
}