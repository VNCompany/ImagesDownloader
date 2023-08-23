using System;

namespace XPathParsing
{
    internal static class ThrowHelper
    {
        public static void ThrowInvalidPatternException()
            => throw new InvalidOperationException("Invalid pattern (was been null or empty)");
        
        public static void ThrowInvalidNodeException(int index, string body)
            => throw new InvalidOperationException($"Invalid node with index {index} in the pattern. Body: `{body}`");

        public static void ThrowInvalidNodeValueException(int index, string body)
            => throw new InvalidOperationException($"Invalid node value. Index: {index}, Body: `{body}`");

        public static void ThrowInvalidNodeFilterException(int index, string body)
            => throw new InvalidOperationException($"Invalid node filter. Index: {index}, Body: `{body}`");
    }
}