using System;
using System.Collections.Generic;
using System.Text;

namespace Sol_Script
{
    enum TokenType
    {
        // Literals
        NUMBER,

        // Single Character Tokens
        PLUS, MINUS, DIVIDE, MULTIPLY, ASSIGN, LEFT_BRACKET, RIGHT_BRACKET, GREATER, LESS,

        // Two Character Tokens
        EQUALITY, GREATER_OR_EQUAL, LESS_OR_EQUAL
    }

    class Token
    {
        public TokenType Type { get; set; }
        public string TokenValue { get; set; }

        public Token(TokenType type, string tokenValue)
        {
            Type = type;
            TokenValue = tokenValue;
        }
    }
}
