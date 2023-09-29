using System;
using System.Collections.Generic;

using Parser = XPathParsing.Internal.XPathElementParser;

namespace XPathParsing
{
    public abstract class XPathConditionBase
    {
        public abstract IEnumerable<T> Apply<T>(XPathConditionArgs<T> args);
        
        internal static XPathConditionBase Create(ReadOnlySpan<Parser.Token> tokens)
        {
            if (tokens.Length == 1)
                return new XPathConditionIndex(tokens[0]);

            return new XPathConditionFilter(tokens);
        }
    }
}