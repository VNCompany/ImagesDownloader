namespace XPathParsing.Internal
{
    class XPathProviderDefault : IXPathProvider
    {
        private string? value;

        private XPathProviderDefault(object? obj)
        {
            value = obj?.ToString();
        }

        public bool ProcessCondition(XPathConditionFilter condition)
            => value != null && condition.Predicate.Invoke(value);

        public static IXPathProvider GetProvider<T>(T element) => new XPathProviderDefault(element);
    }
}