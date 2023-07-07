using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace HtmlParsing
{
    public class HtmlParser
    {
        private enum TagType : byte { StartTag = 1, EndTag = 2, Invalid = 4 }
        private ref struct TagFrame
        {
            public TagFrame() { }
            public TagType Type = TagType.Invalid;
            public int Start = -1;
            public int Length = 0;
        }
        
        private readonly string html;
        private readonly List<HtmlTag> tags;

        public IList<HtmlTag> Tags => tags;

        public HtmlParser(string html, bool onlyHead = false)
        {
            tags = new List<HtmlTag>();
            this.html = html;
            
            LoadTags(onlyHead);
        }

        private void LoadTags(bool onlyHead)
        {
            int ptr = 0;
            bool stop = false;
            while (ptr < html.Length && stop == false)
            {
                var tagFrame = ParseTagFrame(html, ptr);

                switch (tagFrame.Type)
                {
                    case TagType.StartTag:
                    {
                        TagSchema tagSchema = new TagSchema(html, tagFrame.Start, tagFrame.Length);
                        tags.Add(new HtmlTag(tagSchema, tagSchema.GetNameSpan().ToString().ToLower()));
                        ptr = tagSchema.Start + tagSchema.Length;
                    }
                        break;

                    case TagType.EndTag:
                    {
                        TagSchema tagSchema = new TagSchema(html, tagFrame.Start, tagFrame.Length, 1);
                        int startTagIndex = tags.FindLastIndex(tag => tag.Schema.NameEquals(tagSchema.GetNameSpan()));
                        if (startTagIndex != -1)
                        {
                            var startTag = tags[startTagIndex];

                            for (int i = startTagIndex + 1; i < tags.Count; i++)
                            {
                                var subTag = tags[i];
                                subTag.Parent ??= startTag;
                            }

                            int contentStart = startTag.Schema.Start + startTag.Schema.Length;
                            int contentLength = tagSchema.Start - contentStart;

                            if (contentLength > 0)
                                startTag.Content = new HtmlContent(html, contentStart, contentLength);

                            if (onlyHead && startTag.Name.Equals("head"))
                                stop = true;
                        }
                        
                        ptr = tagSchema.Start + tagSchema.Length;
                    }
                        break;

                    case TagType.Invalid:
                    default:
                        ptr++;
                        continue;
                }
            }
        }

        private static TagFrame ParseTagFrame(string html, int start)
        {
            int ptr = start;
            while (ptr < html.Length)
            {
                if (html[ptr] == '<' && ptr + 2 < html.Length)
                {
                    start = ptr++;
                    
                    if (html[ptr] == '!')
                    {
                        ptr = ParserExtensions.SkipComment(html, ptr);
                        continue;
                    }
                    
                    TagType tagType = ParserExtensions.IsAsciiAlphaNumerics(html[ptr])
                        ? TagType.StartTag
                        : html[ptr] switch
                        {
                            '/' => TagType.EndTag,
                            _ => TagType.Invalid
                        };
                    
                    if ((byte)tagType < 4)
                    {
                        var tagNative = new TagFrame { Type = tagType, Start = start};
                        for (; ptr < html.Length; ptr++)
                        {
                            if (html[ptr] == '\\')
                                ptr++;
                            else if (ParserExtensions.IsQuote(html, ptr, 0, out int skipLength))
                                ptr += skipLength;
                            else if (html[ptr] == '>')
                            {
                                tagNative.Length = ptr - start + 1;
                                return tagNative;
                            }
                        }
                    }
                } 
                
                ptr++;
            }

            return new() { Type = TagType.Invalid, Start = -1, Length = 0};
        }
    }
}