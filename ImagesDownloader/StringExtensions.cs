using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ImagesDownloader
{
    internal static class StringExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsQuoteChar(char ch) => ch is '"' or '\'';
        
        public static IEnumerable<string> SmartSplit(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) yield break;

            var p = 0;
            while (p < str.Length)
            {
                if (char.IsWhiteSpace(str[p]))
                {
                    ++p;
                    continue;
                }

                var start = p;
                var end = start + 1;
                if (IsQuoteChar(str[start]))
                {
                    char qch = str[start++];
                    for (; end < str.Length && str[end] != qch; end++) { }
                }
                else
                {
                    for (; end < str.Length && !char.IsWhiteSpace(str[end]); end++) { }
                }

                yield return str.Substring(start, end - start);
                p = end + 1;
            }
        }
    }
}