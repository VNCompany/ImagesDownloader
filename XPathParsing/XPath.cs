using System;

namespace XPathParsing
{
    public class XPath
    {
        public XPath(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));

            string[] nodes = value.Split('/');
            if (nodes.Length < 2) throw new InvalidXPathException();
            
            

            // /body/main/div[div^='test']//a[@class='link']/@href
        }
    }
}