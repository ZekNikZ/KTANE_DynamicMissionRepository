using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DMG.Parser
{
    class Lexer
    {
        private static readonly Dictionary<char, TokenType> TOKEN_TYPES = new Dictionary<char, TokenType>
        {
            { '*' , TokenType.OP_REPEAT },
            { ':' , TokenType.OP_VALUE_ASSIGNMENT },
            { ',' , TokenType.OP_ITEM_SEPARATOR },
            { '!' , TokenType.OP_DUPLICATE_REDUCTION_FLAG },
            { '(' , TokenType.OP_BOMB_GROUP_OPEN },
            { ')' , TokenType.OP_BOMB_GROUP_CLOSE },
            { '[' , TokenType.OP_MODULE_GROUP_OPEN },
            { ']' , TokenType.OP_MODULE_GROUP_CLOSE },
            { ';' , TokenType.OP_STATEMENT_SEPARATOR },
            { '\n' , TokenType.OP_STATEMENT_SEPARATOR },
        };

        public List<Token> LexTokens(List<RawToken> tokens)
        {
            var result = new List<Token>();

            foreach (var token in tokens)
            {
                var lineNumber = token.LineNumber;

                switch (token.TokenType)
                {
                    case RawTokenType.OPERATOR:
                        {
                            if (TOKEN_TYPES.TryGetValue(token.Value[0], out TokenType type))
                            {
                                result.Add(new Token(type, lineNumber));
                            }
                            else
                            {
                                throw new LexerException(token, "Invalid operator detected");
                            }

                            break;
                        }

                    case RawTokenType.VALUE:
                        if (int.TryParse(token.Value, out int intValue))
                        {
                            result.Add(new Token(TokenType.VALUE_INT, lineNumber, intValue));
                        }
                        else if (float.TryParse(token.Value, out float floatValue))
                        {
                            result.Add(new Token(TokenType.VALUE_FLOAT, lineNumber, floatValue));
                        }
                        else
                        {
                            result.Add(new Token(TokenType.VALUE_STRING, lineNumber, token.Value));
                        }
                        break;

                    case RawTokenType.COMMENT:
                        if (token.Value.StartsWith("//"))
                        {
                            result.Add(new Token(TokenType.COMMENT_INLINE, lineNumber, token.Value));
                        }
                        else
                        {
                            result.Add(new Token(TokenType.COMMENT_MULTILINE, lineNumber, token.Value));
                        }
                        break;

                    default:
                        throw new LexerException(token, "Invalid raw token type");
                }
            }

            return result;
        }
    }
}
