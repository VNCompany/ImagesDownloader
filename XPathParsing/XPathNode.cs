using System.Collections.Generic;

using XPathParsing.Internal;

namespace XPathParsing
{
    public class XPathNode
    {

        enum _TokenType { Id, Operator, Value, Block }
        
        #nullable disable
        class _Token
        {
            public _TokenType Type;
            public string Value;
        }
        #nullable restore

        private static List<_Token> GetNodeTokens(string input)
        {
            var tokens = new List<_Token>();
            for (int i = 0; i != input.Length; ++i)
            {
                switch (input[i])
                {
                    /* Values */
                    case '\'':
                    case '"':
                        if (tokens.Count != 0
                            && tokens[^1].Type is _TokenType.Operator or _TokenType.Id)
                        {
                            int length = StringParserTools.GetQuoteLength(input, i);
                            if (length == -1) throw new InvalidXPathException(input);

                            tokens.Add(
                                new _Token { Type = _TokenType.Value, Value = input.Substring(i + 1, length) });
                            i += length + 1;
                        }
                        else throw new InvalidXPathException(input);
                        break;
                    
                    /* Blocks */
                    case '[':
                    case ']':
                        tokens.Add(
                            new _Token { Type = _TokenType.Block, Value = input[i].ToString() });
                        break;
                    
                    /* Operators */
                    case '%':
                    case '^':
                    case '$':
                        if (tokens.Count != 0
                            && tokens[^1].Type == _TokenType.Id)
                        {
                            tokens.Add(
                                new _Token { Type = _TokenType.Operator, Value = input[i].ToString() });
                        }
                        else throw new InvalidXPathException(input);
                        break;
                    
                    case '=':
                        if (tokens.Count != 0)
                        {
                            switch (tokens[^1].Type)
                            {
                                case _TokenType.Id:
                                    tokens.Add(
                                        new _Token { Type = _TokenType.Operator, Value = input[i].ToString() });
                                    break;
                                case _TokenType.Operator when tokens[^1].Value != "=":
                                    tokens[^1].Value += '=';
                                    break;
                                default:
                                    throw new InvalidXPathException(input);
                            }
                        }
                        break;
                    
                    /* Identificators and other */
                    
                    default:
                        if (char.IsWhiteSpace(input[i])) continue;

                        int idLength = StringParserTools.GetIdLength(input, i);
                        string idValue = idLength > 0
                            ? input.Substring(i, idLength)
                            : throw new InvalidXPathException(input);
                        if (idValue == "@") throw new InvalidXPathException(input);
                        
                        tokens.Add(
                            new _Token { Type = _TokenType.Id, Value = idValue });
                        break;
                }
            }
            
            return tokens;
        }
    }
}