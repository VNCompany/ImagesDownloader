using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using XPathParsing.Internal;

namespace XPathParsing.Internal
{
    class XPathElementParser
    {
        public enum TokenType { Id, Operator, Literal, Block }
        
        public struct Token
        {
            public TokenType Type;
            public string Value;

            public Token(TokenType type, string value)
            {
                Type = type;
                Value = value;
            }
            public Token(TokenType type, char value) : this(type, value.ToString()) { }

            public Token AppendValue(string value)
            {
                Token clone;
                clone.Type = Type;
                clone.Value = string.Concat(Value + value);
                return clone;
            }
        }

        public static List<Token> GetNodeTokens(string input)
        {
            var tokens = new List<Token>();
            for (int i = 0; i != input.Length; ++i)
            {
                switch (input[i])
                {
                    /* Literals */
                    case '\'':
                    case '"':
                        string? value;
                        if (tokens.Count == 0
                            || (tokens[^1].Type != TokenType.Id && tokens[^1].Type != TokenType.Operator)
                            || (value = GetString(input, i)) == null)
                            throw new InvalidXPathException(input, "Incorrect filter value");
                        
                        tokens.Add(new Token(TokenType.Literal, value));
                        i += value.Length + 1;
                    break;
                    
                    /* Blocks */
                    case '[':
                    case ']':
                        tokens.Add(new Token(TokenType.Block, input[i]));
                        break;
                    
                    /* Operators */
                    case '%':
                    case '^':
                    case '$':
                        if (tokens.Count != 0
                            && tokens[^1].Type == TokenType.Id)
                        {
                            tokens.Add(new Token(TokenType.Operator, input[i]));
                        }
                        else throw new InvalidXPathException(input, 
                            "Left operand in the filter is not an identificator");
                        break;
                    
                    case '=':
                        if (tokens.Count != 0)
                        {
                            switch (tokens[^1].Type)
                            {
                                case TokenType.Id:
                                    tokens.Add(new Token(TokenType.Operator, '='));
                                    continue;
                                case TokenType.Operator when tokens[^1].Value != "=":
                                    Token prevOper = tokens[^1];
                                    tokens[^1] = prevOper.AppendValue("=");
                                    continue;
                            }
                        }
                        throw new InvalidXPathException(input, 
                            "Invalid comparsion expression in the filter");
                    
                    /* Identificators and other */
                    default:
                        if (char.IsWhiteSpace(input[i])) continue;

                        string id = GetId(input, i);
                        if (id == String.Empty || (id.Length == 1 && id[0] == '@'))
                            throw new InvalidXPathException(input, "The node has incorrect sequence");
                        
                        tokens.Add(new Token(TokenType.Id, id));
                        i += id.Length - 1;
                        break;
                }
            }
            
            return tokens;
        }

        private static string? GetString(string input, int startIndex)
        {
            char qch = input[startIndex++];
            for (int i = startIndex; i != input.Length; ++i)
            {
                if (input[i] == '\\')
                    i++;
                else if (input[i] == qch) return input.Substring(startIndex, i - startIndex);
            }

            return null;
        }

        private static string GetId(string input, int startIndex)
        {
            int i;
            for (i = startIndex; i != input.Length; ++i)
            {
                if (input[i] == '@')
                {
                    if (i == startIndex) continue;
                    break;
                }
                
                if (!IsIdChar(input[i])) break;
            }

            return i == startIndex ? String.Empty : input.Substring(startIndex, i - startIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsIdChar(char ch) => char.IsAscii(ch) &&
                                                 (char.IsLetterOrDigit(ch) 
                                                  || ch == '_' || ch == '-' || ch == '*' || ch == '.' || ch == ':');
    }
}