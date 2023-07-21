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
                    int match = value.Length - 1;
                    for (int c = i + 1, v = 1; c < content.Length && v < value.Length; c++, v++)
                        if (CharsEquals(content[c], value[v], ignoreCase))
                            match--;

                    if (match == 0)
                        return i;
                }
            }

            return -1;
        }

        public static bool Equals(string content, string value, StringRange range, bool ignoreCase)
        {
            if (range.Length != value.Length || (int)range > content.Length) 
                return false;

            for (int i = 0; i < range.Length; i++)
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
    }
}