using System;
using HtmlParsing.Internal;

namespace HtmlParsing
{
    public class HtmlValue
    {
        private readonly string _content;
        private readonly StringRange _range;
        
        private string _cached;

        public HtmlValue(string content, StringRange range)
        {
            _content = content;
            _range = range;
        }

        public HtmlValue(string content)
        {
            _cached = content;
        }

        public string Value
        {
            get
            {
                if (_cached == null)
                    _cached = _content.Substring(_range.Start, _range.Length);

                return _cached;
            }
        }

        public ReadOnlySpan<char> AsSpan() => _cached ?? _content.AsSpan(_range.Start, _range.Length);
        
        public override string ToString() => Value;
    }
}