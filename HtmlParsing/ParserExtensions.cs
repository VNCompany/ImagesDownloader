using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace HtmlParsing
{
    internal static class ParserExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsQuoteChar(char ch)
        {
            return ch == '"' || ch == '\'';
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsQuote(ReadOnlySpan<char> content, int pointer, int maxLength, out int skipLength)
        {
            if (maxLength == 0) maxLength = content.Length;

            char quote;
            if (IsQuoteChar(quote = content[pointer]))
            {
                int i = pointer + 1;
                for (; i < maxLength && content[i] != quote; i++) { }

                skipLength = i - pointer;
                return true;
            }

            skipLength = 0;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAsciiAlphaNumerics(char ch)
        {
            return char.IsAscii(ch) && (
                (ch >= 'a' && ch <= 'z')
                || (ch >= 'A' && ch <= 'Z')
                || (ch >= '0' && ch <= '9'));
        }

        private static readonly (string, string)[] commentTagKeys =
        {
            ("<!--", "-->"),
            ("<![", "]]>")
        };
        
        public static int SkipComment(string html, int ptr)
        {
            int ei = -1;
            if (ptr + 5 < html.Length)
            {
                var htmlSpan = html.AsSpan(ptr);
                if (htmlSpan.StartsWith(commentTagKeys[0].Item1))
                {
                    ptr += commentTagKeys[0].Item1.Length; 
                    ei = 0;
                }
                else if (htmlSpan.StartsWith(commentTagKeys[1].Item1))
                {
                    ptr += commentTagKeys[1].Item1.Length;
                    ei = 1;
                }
            }

            if (ei != -1)
            {
                for (; ptr + 2 < html.Length; ptr++)
                {
                    if (html.AsSpan(ptr, 3).SequenceEqual(commentTagKeys[ei].Item2))
                        return ptr + 3;
                }
            }

            return ptr + 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NameEquals(this TagSchema schema, ReadOnlySpan<char> other)
            => schema.GetNameSpan().Equals(other, StringComparison.OrdinalIgnoreCase);

        private static KeyValuePair<string, string> ParseAttribute(ReadOnlySpan<char> span)
        {
            var sepIndex = span.IndexOf('=');

            if (sepIndex == -1 || sepIndex + 1 >= span.Length)
            {
                string attr = span.ToString();
                return new KeyValuePair<string, string>(attr, attr);
            }

            string attrName = span.Slice(0, sepIndex).ToString();

            sepIndex++;

            if (sepIndex + 1 < span.Length
                && IsQuoteChar(span[sepIndex])
                && IsQuoteChar(span[^1]))
            {
                sepIndex++;
                return new KeyValuePair<string, string>(
                    attrName, 
                    span.Slice(sepIndex, span.Length - sepIndex - 1).ToString());
            }

            return new KeyValuePair<string, string>(
                attrName,
                span.Slice(sepIndex).ToString());
        }

        public static IEnumerable<KeyValuePair<string, string>> ParseTagAttributes(ReadOnlySpan<char> tagBody)
        {
            var list = new List<KeyValuePair<string, string>>();
            
            for (int i = 0; i < tagBody.Length; i++)
            {
                if (!char.IsWhiteSpace(tagBody[i]))
                {
                    int start = i;
                    bool ignore = false;
                    while (i < tagBody.Length)
                    {
                        if (tagBody[i] == '\\')
                            i += 1;
                        else if (IsQuote(tagBody, i, 0, out int skipLength))
                            i += skipLength;
                        else if (char.IsWhiteSpace(tagBody[i]) && !ignore)
                            break;

                        i += 1;
                    }

                    if (i != start)
                        list.Add(ParseAttribute(tagBody.Slice(start, i - start)));
                }
            }

            return list;
        }
    }
}