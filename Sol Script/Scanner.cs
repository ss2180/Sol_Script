using System;
using System.Collections.Generic;
using System.IO;

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
        public TokenType Type { get; }
        public string TokenValue { get; }

        public Token(TokenType type, string tokenValue)
        {
            this.Type = type;
            this.TokenValue = tokenValue;
        }
    }

    class Scanner
    {
        private List<Token> _tokens = new List<Token>();

        /// <summary>
        /// Scans a string for tokens and stores the tokens in a instanced array.
        /// </summary>
        /// <param name="line"></param>
        /// <exception cref="IOException"></exception>
        public void ScanLine(string line)
        {
            int EOF = line.Length;
            int index = 0;

            while (index < EOF)
            {
                if (line[index] == '-')
                {
                    _tokens.Add(new Token(TokenType.MINUS, "-"));
                    index++;
                }
                else if (line[index] == '+')
                {
                    _tokens.Add(new Token(TokenType.PLUS, "+"));
                    index++;
                }
                else if (line[index] == '*')
                {
                    _tokens.Add(new Token(TokenType.STAR, "*"));
                    index++;
                }
                else if (line[index] == '/')
                {
                    _tokens.Add(new Token(TokenType.SLASH, "/"));
                    index++;
                }
                else if (line[index] == '=')
                {
                    _tokens.Add(new Token(TokenType.EQUALS, "="));
                    index++;
                }
                else if (line[index] == '(')
                {
                    _tokens.Add(new Token(TokenType.LEFT_BRACKET, "("));
                    index++;
                }
                else if (line[index] == ')')
                {
                    _tokens.Add(new Token(TokenType.RIGHT_BRACKET, ")"));
                    index++;
                }
                else
                {
                    if (line[index] == ' ')
                    {
                        index++;
                    }
                    else if (CharIsNumeric(line[index]))
                    {
                        index = ScanToken(line, index);
                    }
                    else
                    {
                        throw new IOException($"Character '{line[index]}' is not a recognised token pattern.");
                    }
                }
            }
        }

        /// <summary>
        /// Scans for the next token starting at the provided index and adds the token to the token list.
        /// </summary>
        /// <returns>The index of the start of the next token.</returns>
        private int ScanToken(string text, int index)
        {
            int nextIndex = index + 1;
            int numberCharLength = 1;

            if (nextIndex < text.Length)
            {
                while (CharIsNumeric(text[nextIndex]))
                {
                    nextIndex++;
                    numberCharLength++;

                    if (nextIndex >= text.Length)
                    {
                        break;
                    }
                }
            }

            string numberString = text.Substring(index, numberCharLength);
            _tokens.Add(new Token(TokenType.NUMBER, numberString));

            return nextIndex;
        }

        private bool CharIsNumeric(char c)
        {
            return c >= '0' && c <= '9';
        }

        public void DisplayTokens()
        {
            foreach (Token token in _tokens)
            {
                string output = $"{"<" + token.Type + ">",15}{token.TokenValue,15}";

                Console.WriteLine(output);
            }
        }

        public List<Token> Tokens
        {
            get { return _tokens; }
        }
    }
}
