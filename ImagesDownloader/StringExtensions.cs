using System.Linq;
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

            int p = 0;
            while (p < str.Length)
            {
                if (IsQuoteChar(str[p]))
                {
                    char qch = str[p];
                    int pp = ++p;
                    while (pp < str.Length && str[pp] != qch)
                    {
                        if (str[pp] == '\\') ++pp;
                        ++pp;
                    }

                    yield return str.Substring(p, pp - p);
                    p = pp;
                    if (pp != str.Length) ++p;
                }
                else if (char.IsWhiteSpace(str[p]))
                    ++p;
                else
                {
                    int pp = p + 1;
                    while (pp < str.Length && char.IsWhiteSpace(str[pp]) == false) ++pp;
                    yield return str.Substring(p, pp - p);
                    p = pp;
                }
            }
        }
    }
}