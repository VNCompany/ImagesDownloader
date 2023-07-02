using System;
using System.Diagnostics.CodeAnalysis;

namespace HtmlParsing
{
    public class HtmlParser
    {
        public struct TagInfo
        {
            public int Position { get; set; } = -1;
            public int Length { get; set; } = 0;
            public string Name { get; set; } = string.Empty;
            public bool IsClosingTag { get; set; } = false;

            public TagInfo(int position, int length, string name, bool isClosingTag)
            {
                Position = position;
                Length = length;
                Name = name;
                IsClosingTag = isClosingTag;
            }
        }
        
        private readonly StringPart html;

        private HtmlParser(StringPart html)
        {
            this.html = html;
        }

        private static bool IsQuoteSign(char ch) => ch == '"' || ch == '\'';
        
        /// <summary>
        /// Get tag position 
        /// </summary>
        /// <param name="content">html text</param>
        /// <param name="tag">lower-case tag name</param>
        /// <returns>Start tag position with sign, returns -1 if not found</returns>
        public static int FindTag(ReadOnlySpan<char> content, string tag)
        {
            int i = 0;
            while (i < content.Length)
            {
                switch (content[i])
                {
                    case '"':
                    case '\'':
                        i++;
                        while (i < content.Length && !IsQuoteSign(content[i]))
                            i++;
                        i++;
                        break;

                    case '<':
                    {
                        i++;
                        int j = 0;
                        while (j < tag.Length && i < content.Length)
                        {
                            if (tag[j] != content[i]
                                && tag[j] != char.ToLower(content[i]))
                                break;
                            i++; j++;
                        }

                        if (j == tag.Length)
                            return i - j - 1;
                    }
                        break;
                    
                    default:
                        i++;
                        break;
                }
            }

            return -1;
        }
        
        /// <param name="htmlPart"></param>
        /// <param name="tagBody">tag body without frames</param>
        /// <returns>true if found</returns>
        public static bool GetTagBody(StringPart htmlPart, [NotNullWhen(true)] out StringPart? tagBody)
        {
            bool ignore = false;
            int start = -1;

            for (int i = 0; i < htmlPart.Length; i++)
            {
                if (IsQuoteSign(htmlPart[i])) ignore = !ignore;

                if (!ignore && htmlPart[i] == '<')
                {
                    start = i + 1;
                    break;
                }
            }

            if (start != -1 && start < htmlPart.Length)
            {
                for (int i = start + 1; i < htmlPart.Length; i++)
                {
                    if (IsQuoteSign(htmlPart[i])) ignore = !ignore;

                    if (!ignore && htmlPart[i] == '>')
                    {
                        tagBody = htmlPart.Slice(start, i - start);
                        return true;
                    }
                }

                tagBody = htmlPart.Slice(start);
                return true;
            }

            tagBody = null;
            return false;
        }

        public static StringPart? GetTagName(StringPart tagBody)
        {
            if (tagBody.Length == 0) return null;

            int i = 0;
            while (i < tagBody.Length 
                   && tagBody[i] != '/' 
                   && char.IsWhiteSpace(tagBody[i]) == false)
                i++;

            return i == 0 ? null : tagBody.Slice(0, i);
        }

        /// <summary>
        /// Get tag info
        /// </summary>
        /// <param name="content">html text</param>
        /// <returns>tag info</returns>
        public static TagInfo GetTag(ReadOnlySpan<char> content)
        {
            int i = 0;
            int start = -1;
            int end = -1;
            bool quote = false;
            
            // Search tag start
            while (i < content.Length)
            {
                if (IsQuoteSign(content[i]))
                {
                    quote = !quote;
                    i++;
                    continue;
                }

                if (!quote && content[i] == '<')
                {
                    start = i;
                    i++;
                    break;
                }
            }
            
            // Parse tag name
            if (start != -1 && i + 2 < content.Length)
            {
                TagInfo ti = new TagInfo()
                {
                    Position = start
                };
                
                // Iterating tag name
                if (content[i + 1] == '/')
                {
                    i++;
                    ti.IsClosingTag = true;
                }
                
                while (i < content.Length
                       && content[i] != '>'
                       && content[i] != '/'
                       && !char.IsWhiteSpace(content[i]))
                    i++;
                ti.Name = content.Slice(start + 1, i - start - 1).ToString();

                while (true)
                {
                    if (i == content.Length)
                    {
                        ti.Length = content.Length - start;
                        break;
                    }

                    if (IsQuoteSign(content[i])) quote = !quote;

                    if (!quote && content[i] == '>')
                    {
                        ti.Length = i - start + 1;
                        break;
                    }
                    
                    i++;
                }

                return ti;
            }
            return default;
        }

        // public static HtmlParser? ParseHead(string html)
        // {
        //     
        // }
        //
        // public static HtmlParser? ParseBody(string html)
        // {
        //     
        // }
    }
}