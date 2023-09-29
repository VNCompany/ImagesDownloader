namespace XPathParsing
{
    public class XPathName
    {
        public bool IsAttributeName { get; }
        
        public string Value { get; }
    
        public XPathName(string value)
        {
            IsAttributeName = value.StartsWith('@');
            Value = IsAttributeName ? value.Substring(1) : value;
        }
    }
}