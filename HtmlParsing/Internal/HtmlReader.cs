using System;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace HtmlParsing.Internal
{
    public struct TagResult
    {
        public bool Success { get; }
        public StringRange Range { get; }

        public TagResult(bool success, StringRange range)
        {
            Success = success;
            Range = range;
        }
    }
    
    public class HtmlReader
    {
        private readonly string _html;
        private int _pos;

        public char this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _html[index];
        }

        public HtmlReader(string html)
        {
            if (string.IsNullOrEmpty(html))
                throw new ArgumentException("html");

            _html = html;
            _pos = 0;
        }
        
        private int SkipQuote(int startIndex)
        {
            char ch = _html[startIndex++];
            while (startIndex < _html.Length && _html[startIndex] != ch)
                startIndex++;
            return startIndex;
        }
        
        private void SkipMeta()
        {
            int index = _pos;
            if (StringTools.Equals(_html, "<!--", new StringRange(_pos, 4), false))
                index = StringTools.IndexOf(_html, "-->", _pos + 4, false);
            else if (StringTools.Equals(_html, "<![CDATA[", new StringRange(_pos, 9), true))
                index = StringTools.IndexOf(_html, "]]>", _pos + 9, false);

            if (index == -1)
                _pos = _html.Length - 1;
            else
                _pos = index;
        }

        private int IndexOfWithIgnoreQuotes(char ch, int startIndex = 0)
        {
            for (int i = startIndex; i < _html.Length; i++)
            {
                if (_html[i] == ch)
                    return i;
                
                if (StringTools.IsQuoteChar(_html[i]))
                    i = SkipQuote(i);
            }

            return -1;
        }
        
        /// <returns>(bool Success, StringRange)</returns>
        public TagResult GetTag()
        {
            while (_pos < _html.Length 
                   && (_pos = _html.IndexOf('<', _pos)) != -1
                   && _pos + 2 < _html.Length)
            {
                if (_html[_pos + 1] == '!')
                {
                    SkipMeta();
                    _pos++;
                    continue;
                }

                int closeTagIndex = IndexOfWithIgnoreQuotes('>', _pos + 1);
                if (closeTagIndex == -1) break;

                int startIndex = _pos;
                _pos = closeTagIndex + 1;
                return new TagResult(true, new StringRange(startIndex, closeTagIndex - startIndex + 1));
            }
            
            return default;
        }

        public StringRange GetTagName(StringRange tagRange, bool isCloseTag)
        {
            tagRange.Length -= 1;
            tagRange = isCloseTag ? tagRange >> 2 : tagRange >> 1;

            if (tagRange.Length > 0 && StringTools.IsAsciiAlphaChar(_html[tagRange.Start]))
            {
                int index = tagRange.Start;
                while (index < (int)tagRange && StringTools.IsAsciiAlphaOrDigitChar(_html[index])) index++;
                return new StringRange(tagRange.Start, index - tagRange.Start);
            }

            return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => _html;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(StringRange range) => _html.Substring(range.Start, range.Length);
    }
}