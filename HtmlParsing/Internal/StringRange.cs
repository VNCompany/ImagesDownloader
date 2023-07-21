using System.Runtime.CompilerServices;

namespace HtmlParsing.Internal
{
    public struct StringRange
    {
        public int Start = -1;
        public int Length = 0;

        public StringRange(int start, int length)
        {
            Start = start;
            Length = length;
        }
        
        public static StringRange operator >> (StringRange range, int offset)
        {
            int newStart = range.Start + offset;
            return new StringRange(newStart, range.Length - offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator int(StringRange range) => range.Start + range.Length;
    }
}