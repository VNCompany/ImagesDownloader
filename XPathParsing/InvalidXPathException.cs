using System;

namespace XPathParsing
{
    public class InvalidXPathException : Exception
    {
        public InvalidXPathException() : base("Invalid XPath sequence") { }

        public InvalidXPathException(string node) : base($"Invalid XPath sequence `{node}`") { }
        
        public InvalidXPathException(string node, string message) 
            : base($"Invalid XPath sequence `{node}`: {message}") { }
    }
}