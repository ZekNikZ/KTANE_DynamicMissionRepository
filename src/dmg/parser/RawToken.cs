using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DMG.Parser {
    public enum RawTokenType {
        VALUE,
        OPERATOR,
        COMMENT
    }

    public class RawToken {
        public RawTokenType TokenType { get; }
        public string Value { get; }

        public int LineNumber { get; }
        public RawToken(string value, RawTokenType type, int lineNumber) {
            Value = value;
            TokenType = type;
            LineNumber = lineNumber;
        }

        public override string ToString() {
            return $"Token{{ Value = {Value}, Type = {TokenType} }}";
        }
    }
}
