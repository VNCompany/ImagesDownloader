using System;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace HtmlParsing.Internal
{
    public class HtmlReader
    {
        private readonly string _html;
        private int _pos;

        private void SkipMeta()
        {
            int index = _pos;
            if (StringTools.Equals(_html, "<!--", new StringRange(_pos, 4), false))
                index = StringTools.IndexOf(_html, "-->", _pos + 4, false);
            else if (StringTools.Equals(_html, "<![CDATA[", new StringRange(_pos, 9), true))
                index = StringTools.IndexOf(_html, "]]>", _pos + 9, false);

            if (index == -1)
                _pos = _html.Length;
            else
                _pos = index + 1;
        }
        
        private int GoToEndQuote(int startIndex)
        {
            char ch = _html[startIndex++];
            while (startIndex < _html.Length && _html[startIndex] != ch)
                startIndex++;
            return startIndex;
        }

        private int IndexOfWithIgnoreQuotes(char ch, int startIndex)
        {
            for (int i = startIndex; i < _html.Length; i++)
            {
                if (_html[i] == ch)
                    return i;
                
                if (StringTools.IsQuoteChar(_html[i]))
                    i = GoToEndQuote(i);
            }

            return -1;
        }
        
        private StringRange GetTagNameRange(StringRange tagRange, bool isCloseTag)
        {
            // Получение тела тега
            tagRange = isCloseTag ? tagRange >> 2 : tagRange >> 1;
            tagRange.Length--;
            
            if (tagRange.Length > 0 && StringTools.IsAsciiAlphaChar(_html[tagRange.Start]))
            {
                int index = tagRange.Start + 1;
                while (index < (int)tagRange && StringTools.IsAsciiAlphaOrDigitChar(_html[index])) index++;
                return new StringRange(tagRange.Start, index - tagRange.Start);
            }

            return default;
        }

        public StringRange FindCloseTag(string name, int startIndex)
        {
            int start = -1;
            while (startIndex < _html.Length)
            {
                if (start != -1)
                {
                    if (_html[startIndex] == '>')
                        return new StringRange(start, startIndex - start + 1);
                    else if (char.IsWhiteSpace(_html[startIndex]) == false)
                        start = -1;
                    
                    startIndex++;
                    continue;
                }

                startIndex = _html.IndexOf('<', startIndex);
                if (startIndex == -1 || startIndex + 1 >= _html.Length) break;

                if (_html[startIndex + 1] == '/'
                    && StringTools.Equals(_html, name, new StringRange(startIndex + 2, name.Length), true))
                {
                    start = startIndex;
                    startIndex += name.Length + 2;
                }
                else
                    startIndex++;
            }
            return new StringRange(_html.Length, 0);
        }
        
        public string Content
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _html;
        }

        public HtmlReader(string html)
        {
            if (string.IsNullOrEmpty(html))
                throw new ArgumentException("html");

            _html = html;
            _pos = 0;
        }
        
        public bool ReadTag(out TagInfo tagInfo)
        {
            while (_pos < _html.Length
                   && (_pos = _html.IndexOf('<', _pos)) != -1
                   && _pos + 2 < _html.Length)
            {
                if (_html[_pos + 1] == '!')
                {
                    SkipMeta();
                    continue;
                }

                bool isCloseTag = _html[_pos + 1] == '/';
                int lastIndex = IndexOfWithIgnoreQuotes('>', _pos);

                if (lastIndex != -1)
                {
                    StringRange tagRange = new StringRange(_pos, lastIndex - _pos + 1);
                    StringRange tagNameRange = GetTagNameRange(tagRange, isCloseTag);
                    
                    _pos = lastIndex + 1;

                    if (tagNameRange.Length > 0)
                    {
                        if (!isCloseTag && (_html[tagNameRange.Start] == 's'
                                            || _html[tagNameRange.Start] == 'S'))
                        {
                            if (StringTools.Equals(_html, "script", tagNameRange, true))
                                _pos = FindCloseTag("script", _pos).Start;
                            else if (StringTools.Equals(_html, "style", tagNameRange, true))
                                _pos = FindCloseTag("style", _pos).Start;
                        }
                        
                        tagInfo = new TagInfo(tagRange, tagNameRange, isCloseTag);
                        return true;
                    }
                }
                
                break;
            }

            tagInfo = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetString(StringRange range) => _html.Substring(range.Start, range.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => _html;
    }
}