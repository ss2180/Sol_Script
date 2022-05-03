﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Sol_Script
{
    enum TokenType
    {
        // Literals
        INT_NUM, FLOAT_NUM, BOOL, STRING,

        // Single Character Tokens
        PLUS, MINUS, DIVIDE, MULTIPLY, ASSIGN, LEFT_BRACKET, RIGHT_BRACKET, GREATER, LESS, NOT, NEGATE,

        // Two Character Tokens
        EQUAL, NOTEQUAL, GREATER_OR_EQUAL, LESS_OR_EQUAL,

        //Identifier
        IDENTIFIER,

        //Keywords
        PRINT
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
