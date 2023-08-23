using System;

namespace XPathParsing
{
    public enum XPathSearchOption { Equality, StartsWith, EndsWith, Contains }
    
    public class XPathFilter : IXPathFilter
    {
        public XPathValue Name { get; }
        public string Value { get; }
        public bool CaseInsensitive { get; }
        public XPathSearchOption Option { get; }
        
        public XPathFilter(XPathValue name, string value, bool caseInsensitive, XPathSearchOption option)
        {
            Name = name;
            Value = value;
            CaseInsensitive = caseInsensitive;
            Option = option;
        }

        public bool Validate(string input)
        {
            string value;
            if (CaseInsensitive)
            {
                input = input.ToUpper();
                value = Value.ToUpper();
            }
            else
                value = Value;

            return Option switch
            {
                XPathSearchOption.Equality => value == input,
                XPathSearchOption.StartsWith => value.StartsWith(input),
                XPathSearchOption.EndsWith => value.EndsWith(input),
                XPathSearchOption.Contains => value.Contains(input),
                _ => false
            };
        }

        public static XPathFilter? Parse(ReadOnlySpan<char> filterSpan)
        {
            filterSpan = filterSpan.Trim();
            int eqIndex = filterSpan.IndexOf('=');
            if (eqIndex == 0 || eqIndex + 1 == filterSpan.Length)
                return null;

            bool caseInsensetive = filterSpan[eqIndex + 1] == 'i';
            
            int leftPartMargin = caseInsensetive ? 1 : 0;
            if (eqIndex + 2 + leftPartMargin >= filterSpan.Length
                || filterSpan[eqIndex + 1 + leftPartMargin] != '\''
                || filterSpan[^1] != '\'')
                return null;

            int valueStart = eqIndex + 2 + leftPartMargin;
            ReadOnlySpan<char> valueSpan = filterSpan.Slice(valueStart, filterSpan.Length - valueStart - 1);

            XPathSearchOption option = GetOptionByChar(filterSpan[eqIndex - 1]);
            int nameLength = option == XPathSearchOption.Equality ? eqIndex : eqIndex - 1;

            XPathValue? name = nameLength > 0 ? XPathValue.Parse(filterSpan.Slice(0, nameLength)) : null;

            return name != null ? new XPathFilter(name, valueSpan.ToString(), caseInsensetive, option) : null; 
        }

        private static XPathSearchOption GetOptionByChar(char ch) => ch switch
        {
            '^' => XPathSearchOption.StartsWith,
            '$' => XPathSearchOption.EndsWith,
            '%' => XPathSearchOption.Contains,
            _ => XPathSearchOption.Equality
        };

        public override string ToString()
        {
            string name = Name.ToString();
            if (CaseInsensitive)
                name = $"ToUpper({name})";

            string value = CaseInsensitive ? $"\"{Value.ToUpper()}\"" : $"\"{Value}\"";

            return Option switch
            {
                XPathSearchOption.Equality => $"Filter({name} == {value})",
                XPathSearchOption.StartsWith => $"Filter({name}.StartsWith({value}))",
                XPathSearchOption.EndsWith => $"Filter({name}.EndsWith({value}))",
                XPathSearchOption.Contains => $"Filter({name}.Contains({value}))",
                _ => "Filter()"
            };
        }
    }
}