namespace DMG.Parser {
    public enum TokenType {
        OP_REPEAT,
        OP_VALUE_ASSIGNMENT,
        OP_ITEM_SEPARATOR,
        OP_DUPLICATE_REDUCTION_FLAG,
        OP_BOMB_GROUP_OPEN,
        OP_BOMB_GROUP_CLOSE,
        OP_MODULE_GROUP_OPEN,
        OP_MODULE_GROUP_CLOSE,
        OP_STATEMENT_SEPARATOR,
        VALUE_STRING,
        VALUE_INT,
        VALUE_FLOAT,
        COMMENT_INLINE,
        COMMENT_MULTILINE
    }

    public class Token {
        public TokenType TokenType { get; }

        public int LineNumber { get; }

        public string StringValue { get; }

        public float? FloatValue { get; }

        public int? IntValue { get; }

        public float? NumericValue => FloatValue != null ? FloatValue : IntValue;

        public Token(TokenType type, int lineNumber) : this(type, lineNumber, null, null, null) { }

        public Token(TokenType type, int lineNumber, string value) : this(type, lineNumber, value, null, null) { }

        public Token(TokenType type, int lineNumber, int value) : this(type, lineNumber, null, value, null) { }

        public Token(TokenType type, int lineNumber, float value) : this(type, lineNumber, null, null, value) { }

        private Token(TokenType type, int lineNumber, string stringValue, int? intValue, float? floatValue) {
            TokenType = type;
            LineNumber = lineNumber;
            StringValue = stringValue;
            IntValue = intValue;
            FloatValue = floatValue;
        }

        public override string ToString() {
            if (StringValue != null) {
                return $"Token{{ Type = {TokenType}, LineNumber = {LineNumber}, StringValue = '{StringValue}' }}";
            } else if (IntValue != null) {
                return $"Token{{ Type = {TokenType}, LineNumber = {LineNumber}, IntValue = {IntValue} }}";
            } else if (FloatValue != null) {
                return $"Token{{ Type = {TokenType}, LineNumber = {LineNumber}, FloatValue = {FloatValue} }}";
            } else {
                return $"Token{{ Type = {TokenType}, LineNumber = {LineNumber} }}";
            }
        }
    }
}
