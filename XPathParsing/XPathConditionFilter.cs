using System;
using System.Linq;
using System.Collections.Generic;

using Parser = XPathParsing.Internal.XPathElementParser;

namespace XPathParsing
{
    public sealed class XPathConditionFilter : XPathConditionBase
    {
        public XPathName ElementName { get; }
        
        public bool IgnoreCase { get; } = false;
        
        public string ExpectedValue { get; }

        public Func<string, bool> Predicate { get; }
        
        internal XPathConditionFilter(ReadOnlySpan<Parser.Token> tokens)
        {
            if (tokens.Length < 3
                || tokens[0].Type != Parser.TokenType.Id
                || tokens[1].Type != Parser.TokenType.Operator
                || tokens[^1].Type != Parser.TokenType.Literal
                || (tokens.Length == 4 && tokens[2].Value != "i"))
                throw new InvalidXPathException("Invalid filter body");

            IgnoreCase = tokens.Length == 4;
            ElementName = new XPathName(tokens[0].Value);
            ExpectedValue = tokens[^1].Value;
            if (IgnoreCase) 
                ExpectedValue = ExpectedValue.ToUpper();
            
            Predicate = tokens[1].Value switch
            {
                "^=" => s => (IgnoreCase ? s.ToUpper() : s).StartsWith(ExpectedValue),
                "$=" => s => (IgnoreCase ? s.ToUpper() : s).EndsWith(ExpectedValue),
                "%=" => s => (IgnoreCase ? s.ToUpper() : s).Contains(ExpectedValue),
                _ => s => (IgnoreCase ? s.ToUpper() : s).Equals(ExpectedValue)
            };
        }

        public override IEnumerable<T> Apply<T>(XPathConditionArgs<T> args)
            => args.Collection.Where(t => args.Predicate?.Invoke(t, this) ?? false);
    }
}