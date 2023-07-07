using System;
using System.Runtime.CompilerServices;

namespace HtmlParsing
{
    public class TagSchema
    {
        private readonly string html;
        private Tuple<int, int> nameSpan = null!;

        private void ParseName(int offset)
        {
            int start = BodyStart + offset;
            int fullLength = BodyStart + BodyLength;
            int ptr = start;
            while (ptr < fullLength
                   && ParserExtensions.IsAsciiAlphaNumerics(html[ptr]))
                ptr++;

            nameSpan = new Tuple<int, int>(start, ptr - start);
        }
        
        public int Start { get; }
        public int Length { get; }

        public int BodyStart => Start + 1;
        public int BodyLength => Length - 2;

        public TagSchema(string html, int start, int length, int nameOffset = 0)
        {
            this.html = html;
            Start = start;
            Length = length;
            
            ParseName(nameOffset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<char> GetBodySpan() => html.AsSpan(BodyStart, BodyLength);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<char> GetNameSpan() => html.AsSpan(nameSpan.Item1, nameSpan.Item2);

        public override string ToString() => html.Substring(Start, Length);
    }
}