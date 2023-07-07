using System;
using System.Runtime.CompilerServices;

namespace HtmlParsing
{
    public class HtmlContent
    {
        private readonly string html;
        
        public int Start { get; }
        public int Length { get; }

        public HtmlContent(string html, int start, int length)
        {
            this.html = html;
            Start = start;
            Length = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<char> AsSpan() => html.AsSpan(Start, Length);

        public override string ToString() => html.Substring(Start, Length);
    }
}