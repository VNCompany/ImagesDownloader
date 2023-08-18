using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace HtmlParsing.Internal
{
    internal static class StringTools
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool CharsEquals(char ch1, char ch2, bool ignoreCase) =>
            ch1 == ch2 || (ignoreCase && char.ToUpper(ch1) == char.ToUpper(ch2));
        
        public static int IndexOf(string content, string value, int startIndex, bool ignoreCase)
        {
            for (int i = startIndex; i < content.Length; i++)
            {
                if (CharsEquals(content[i], value[0], ignoreCase))
                {
                    int contentIndex = i + 1;
                    int valueIndex = 1;
                    while (contentIndex < content.Length
                           && valueIndex < value.Length
                           && CharsEquals(content[contentIndex], value[valueIndex], ignoreCase))
                    {
                        contentIndex++;
                        valueIndex++;
                    }

                    if (valueIndex == value.Length)
                        return i;
                }
            }

            return -1;
        }

        public static bool Equals(string content, string value, StringRange range, bool ignoreCase)
        {
            if ((int)range >= content.Length)
                return false;

            for (int i = 0; i != range.Length; i++)
                if (CharsEquals(content[range.Start + i], value[i], ignoreCase) == false)
                    return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsQuoteChar(char ch) => ch == '"' || ch == '\'';

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAsciiAlphaChar(char ch) => (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAsciiAlphaOrDigitChar(char ch) => IsAsciiAlphaChar(ch) || (ch >= '0' && ch <= '9');

        public static IEnumerable<string> SpaceSplit(string input)
        {
            int i = 0;
            while (i < input.Length)
            {
                if (!char.IsWhiteSpace(input[i]))
                {
                    int j = i;
                    while (j < input.Length && !char.IsWhiteSpace(input[j]))
                        j++;
                    yield return input.Substring(i, j - i);
                    i = j;
                }
                else
                    i++;
            }
        }
    }
}