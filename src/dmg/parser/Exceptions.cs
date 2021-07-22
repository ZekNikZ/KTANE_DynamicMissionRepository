using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DMG
{
    class MissionConstructionException : Exception
    {
        public MissionConstructionException(string message) : base(message) { }
    }
}

namespace DMG.Parser
{
    class LexerException : Exception
    {
        public RawToken Token { get; }

        public LexerException(RawToken token, string message) : base(message + $" ({token})")
        {
            Token = token;
        }
    }
    class ParseException : Exception
    {
        public Token Token { get; }

        public ParseException(Token token, string message) : base(message + (token != null ? $" (line {token.LineNumber}) ({token})" : ""))
        {
            Token = token;
        }
    }
}

namespace DMG.Parser.Validation
{
    class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
    }
}