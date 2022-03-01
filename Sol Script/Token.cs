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
        PLUS, MINUS, SLASH, STAR, EQUALS, LEFT_BRACKET, RIGHT_BRACKET
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
