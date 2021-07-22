using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace DMG.Parser
{
    class Tokenizer
    {
        private const string OPERATOR_CHARS = "*:,!()[];\n";

        public List<RawToken> Tokenize(string source)
        {
            source = source.Replace("\r", "");
            var result = new List<RawToken>();
            var lineNumber = 1;
            string closeTag = null;

            var currentToken = "";
            for (int i = 0; i < source.Length; i++)
            {
                char c = source[i];
                if (closeTag == null && i < source.Length - 1 && c == '/')
                {
                    if (source[i + 1] == '/')
                    {
                        // Inline comment
                        closeTag = "\n";
                    }
                    else if (source[i + 1] == '*')
                    {
                        // Multiline comment
                        closeTag = "*/";
                    }
                }
                else if (closeTag == null && c == '"')
                {
                    closeTag = "\"";
                    continue;
                }
                else if (closeTag != null && c == closeTag[0] && i + closeTag.Length - 1 < source.Length)
                {
                    var valid = true;
                    for (int j = 1; j < closeTag.Length; j++)
                    {
                        if (source[i + j] != closeTag[j])
                        {
                            valid = false;
                            break;
                        }
                    }
                    if (valid)
                    {
                        if (closeTag == "\n" || closeTag == "*/")
                        {
                            currentToken += closeTag;
                            result.Add(new RawToken(currentToken, RawTokenType.COMMENT, lineNumber));
                        }
                        else if (closeTag == "\"")
                        {
                            result.Add(new RawToken(currentToken, RawTokenType.VALUE, lineNumber));
                        }

                        i += closeTag.Length - 1;
                        currentToken = "";
                        closeTag = null;
                        continue;
                    }
                }
                else if (closeTag == null && OPERATOR_CHARS.Contains(c))
                {
                    currentToken = currentToken.Trim();
                    if (!string.IsNullOrEmpty(currentToken))
                    {
                        result.Add(new RawToken(currentToken.Trim(), RawTokenType.VALUE, lineNumber));
                    }
                    result.Add(new RawToken("" + c, RawTokenType.OPERATOR, lineNumber));
                    currentToken = "";

                    if (c == '\n')
                    {
                        ++lineNumber;
                    }
                    continue;
                }

                currentToken += c;
                if (c == '\n')
                {
                    ++lineNumber;
                }
            }

            if (closeTag != null)
            {
                if (closeTag == "\n")
                {
                    result.Add(new RawToken(currentToken, RawTokenType.COMMENT, lineNumber));
                    currentToken = "";
                }
                else if (closeTag == "*/")
                {
                    throw new ParseException(null, "Unclosed multiline comment");
                }
            }

            currentToken = currentToken.Trim();
            if (!string.IsNullOrEmpty(currentToken))
            {
                if (OPERATOR_CHARS.Contains(currentToken))
                {
                    result.Add(new RawToken(currentToken, RawTokenType.OPERATOR, lineNumber));
                }
                else
                {
                    result.Add(new RawToken(currentToken, RawTokenType.VALUE, lineNumber));
                }
            }

            return result;
        }
    }
}
