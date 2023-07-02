using System;
using System.Collections;
using System.Collections.Generic;

namespace HtmlParsing
{
    public class StringPart : IEnumerable<char>
    {
        public string Original { get; }
        public int Start { get; }
        public int Length { get; }

        public StringPart(string original, int? start = null, int? length = null)
        {
            Start = start ?? 0;
            if (Start < 0 || Start >= original.Length) throw new IndexOutOfRangeException(nameof(start));
            
            Length = length ?? original.Length - Start;
            if (Start + Length > original.Length) throw new IndexOutOfRangeException(nameof(length));
            
            Original = original;
        }

        public StringPart(StringPart part, int? start = null, int? length = null)
        {
            int parentMaxLength = part.Start + part.Length;
            Start = part.Start + (start ?? 0);
            if (Start < 0 || Start >= parentMaxLength) throw new IndexOutOfRangeException(nameof(start));
            
            Length = length ?? parentMaxLength - Start;
            if (Start + Length > parentMaxLength) throw new IndexOutOfRangeException(nameof(length));
            
            Original = part.Original;
        }

        public char this[int index]
        {
            get
            {
                if (index < 0 || index >= Length)
                    throw new IndexOutOfRangeException();
                return Original[Start + index];
            }
        }

        public StringPart Slice(int start) => new StringPart(this, start);
        public StringPart Slice(int start, int length) => new StringPart(this, start, length);

        public ReadOnlySpan<char> AsSpan() => Original.AsSpan().Slice(Start, Length);

        public override string ToString() => Original.Substring(Start, Length);

        public static implicit operator StringPart(string value) => new StringPart(value);

        public IEnumerator<char> GetEnumerator() => Original.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}