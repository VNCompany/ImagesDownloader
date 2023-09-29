using System;
using System.Collections.Generic;

namespace XPathParsing
{
    public class XPathConditionArgs<T>
    {
        public IEnumerable<T> Collection { get; }

        public Func<T, XPathConditionFilter, bool>? Predicate { get; }

        public XPathConditionArgs(IEnumerable<T> collection, Func<T, XPathConditionFilter, bool>? predicate = null)
        {
            Collection = collection;
            Predicate = predicate;
        }
    }
}