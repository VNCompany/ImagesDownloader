namespace HtmlParsing.Internal
{
    internal class TagInfo
    {
        public StringRange Range;
        public StringRange NameRange;
        public bool IsCloseTag;

        public TagInfo()
        {
            Range = default;
            NameRange = default;
            IsCloseTag = default;
        }

        public TagInfo(StringRange range, StringRange nameRange, bool isCloseTag)
        {
            Range = range;
            NameRange = nameRange;
            IsCloseTag = isCloseTag;
        }
    }
}