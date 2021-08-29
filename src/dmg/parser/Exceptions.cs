using System;

namespace DMG {
    public class MissionConstructionException : Exception {
        public MissionConstructionException(string message) : base(message) { }
    }
}

namespace DMG.Parser {
    public class LexerException : Exception {
        public RawToken Token { get; }

        public LexerException(RawToken token, string message) : base(message + $" ({token})") {
            Token = token;
        }
    }
    public class ParseException : Exception {
        public Token Token { get; }

        public ParseException(Token token, string message) : base(message + (token != null ? $" (line {token.LineNumber}) ({token})" : "")) {
            Token = token;
        }
    }
}

namespace DMG.Parser.Validation {
    public class ValidationException : Exception {
        public ValidationException(string message) : base(message) { }
    }
}