namespace XPathParsing
{
    public interface IXPathProvider
    {
        bool ProcessCondition(XPathConditionFilter condition);
    }
}