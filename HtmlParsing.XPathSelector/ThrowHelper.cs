using System;

namespace HtmlParsing.XPathSelector
{
    internal static class ThrowHelper
    {
        public static void ThrowInvalidXPathSequence() => throw new InvalidOperationException("Invalid XPath sequence");

        public static InvalidOperationException InvalidXPathSequence(string? msg = null) =>
            new InvalidOperationException("Invalid XPath sequence" +
                                          (msg != null ? string.Concat(": ", msg) : string.Empty));
    }
}