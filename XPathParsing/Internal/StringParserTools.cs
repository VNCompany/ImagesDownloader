using System.Runtime.CompilerServices;

namespace XPathParsing.Internal
{
    static class StringParserTools
    {
        public static int GetQuoteLength(string input, int startIndex)
        {
            char qch = input[startIndex++];
            for (int i = startIndex; i < input.Length; i++)
            {
                if (input[i] == '\\')
                    i++;
                else if (input[i] == qch) return i - startIndex;
            }

            return -1;
        }

        public static int GetIdLength(string input, int startIndex)
        {
            int end = input[startIndex] == '@' ? startIndex + 1 : startIndex;
            while (end != input.Length
                   && char.IsAscii(input[end])
                   && (input[end] == '*'
                       || input[end] == '.'
                       || input[end] == '-'
                       || input[end] == '_'
                       || char.IsLetterOrDigit(input[end])
                       || input[end] == ':')) end++;

            return end - startIndex;
        }
    }
}