using System;
using System.Linq;
using System.Collections.Generic;

using Parser = XPathParsing.Internal.XPathElementParser;

namespace XPathParsing
{
    public sealed class XPathConditionIndex : XPathConditionBase
    {
        private readonly int _index;
        
        internal XPathConditionIndex(Parser.Token token)
        {
            if (!int.TryParse(token.Value, out int result))
                throw new InvalidXPathException(token.Value, "Invalid filter body");

            _index = result;
        }

        public override IEnumerable<T> Apply<T>(IEnumerable<T> collection, Func<T, IXPathProvider>? _ = null)
            => collection.Skip(_index).Take(1);
    }
}