using System;

namespace XPathParsing
{
    public enum XPathValueType { Element, Attribute }
    
    public class XPathValue
    {
        public string Value { get; }
        
        public XPathValueType ValueType { get; }
        
        public XPathValue(string value, XPathValueType valueType)
        {
            Value = value;
            ValueType = valueType;
        }

        public static XPathValue? Parse(ReadOnlySpan<char> value)
        {
            if (value.Length == 0 || value.IsWhiteSpace())
                return null;

            bool hasAtSymbol = value[0] == '@';

            if (hasAtSymbol && value.Length < 2)
                return null;

            return hasAtSymbol
                ? new XPathValue(value.Slice(1).ToString(), XPathValueType.Attribute)
                : new XPathValue(value.ToString(), XPathValueType.Element);
        }

        public override string ToString() => ValueType == XPathValueType.Attribute ? $"Attr[{Value}]" : Value;
    }
}