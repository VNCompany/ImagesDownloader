using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable

namespace HtmlParsing.Internal
{
    internal class HtmlReader
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

        private int GoToEndQuote(int startIndex, int length = 0)
        {
            length = length == 0 ? _html.Length : startIndex + length;
            
            char ch = _html[startIndex++];
            while (startIndex < length && _html[startIndex] != ch)
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

            int length = (int)tagRange;
            if (tagRange.Length > 0 && StringTools.IsAsciiAlphaChar(_html[tagRange.Start]))
            {
                int index = tagRange.Start + 1;
                while (index < length && StringTools.IsAsciiAlphaOrDigitChar(_html[index])) index++;
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
        
        public HtmlReader(string html)
        {
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

        public IReadOnlyList<KeyValuePair<StringRange, StringRange>> ReadTagAttributes(StringRange tagRange, int nameLength)
        {
            StringRange bodyRange = tagRange >> (1 + nameLength);
            bodyRange.Length--;
            var list = new List<KeyValuePair<StringRange, StringRange>>();

            int i = bodyRange.Start;
            int bodyLength = (int)bodyRange;
            while (i < bodyLength)
            {
                if (StringTools.IsAsciiAlphaChar(_html[i]))
                {
                    int j = i + 1;
                    int sep = 0;
                    StringRange valueRange = new StringRange(-1, -1);
                    while (j < bodyLength)
                    {
                        if (_html[j] == '=')
                        {
                            sep = j++;
                            valueRange.Start = j;
                        }
                        else if (StringTools.IsQuoteChar(_html[j]) && sep == j - 1)
                        {
                            valueRange.Start++;
                            j = GoToEndQuote(j, bodyRange.Length);
                            valueRange.Length = j - valueRange.Start;
                            break;
                        }
                        else if (!char.IsWhiteSpace(_html[j]))
                            j++;
                        else break;
                    }

                    StringRange keyRange = new StringRange(i, sep != 0 ? sep - i : j - i);

                    if (valueRange.Length == -1)
                    {
                        if (sep == 0)
                            list.Add(new KeyValuePair<StringRange, StringRange>(keyRange, keyRange));
                        else
                        {
                            valueRange.Length = j - valueRange.Start;
                            list.Add(new KeyValuePair<StringRange, StringRange>(keyRange, valueRange));
                        }
                    }
                    else
                        list.Add(new KeyValuePair<StringRange, StringRange>(keyRange, valueRange));

                    i = j;
                }

                i++;
            }

            return list;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetString(StringRange range) => _html.Substring(range.Start, range.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => _html;
    }
}