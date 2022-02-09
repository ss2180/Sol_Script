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
        PLUS, MINUS, SLASH, STAR, EQUALS
    }

    class Token
    {
        public TokenType type { get; }
        public string tokenValue { get; }

        public Token(TokenType type, string tokenValue)
        {
            this.type = type;
            this.tokenValue = tokenValue;
        }
    }

    class Scanner
    {
        private List<Token> tokens = new List<Token>();
        public void ScanLine(string line)
        {
            int EOF =  line.Length;
            int index = 0;

            while (index < EOF)
            {
                if(line[index] == '-')
                {
                    tokens.Add(new Token(TokenType.MINUS, "-"));
                    index++;
                }
                else if(line[index] == '+')
                {
                    tokens.Add(new Token(TokenType.PLUS, "+"));
                    index++;
                }
                else if (line[index] == '*')
                {
                    tokens.Add(new Token(TokenType.STAR, "*"));
                    index++;
                }
                else if (line[index] == '/')
                {
                    tokens.Add(new Token(TokenType.SLASH, "/"));
                    index++;
                }
                else if (line[index] == '=')
                {
                    tokens.Add(new Token(TokenType.EQUALS, "="));
                    index++;
                }
                else
                {
                    if(line[index] == ' ')
                    {
                        index++;
                    }
                    else
                    {
                        index = ScanToken(line, index);
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

            while (CharIsNumeric(text[nextIndex]))
            {
                nextIndex++;
                numberCharLength++;
            }

            string numberString = text.Substring(index, numberCharLength);
            tokens.Add(new Token(TokenType.NUMBER, numberString));

            return nextIndex;
        }

        private bool CharIsNumeric(char c)
        {
            return c >= '0' && c <= '9';
        }

        public void DisplayTokens()
        {
            foreach(Token token in tokens)
            {
                string output = "";

                switch(token.type)
                {
                    case TokenType.EQUALS:
                        output += "<EQUALS>";
                        break;
                    case TokenType.MINUS:
                        output += "<MINUS>";
                        break;
                    case TokenType.NUMBER:
                        output += "<NUMBER>";
                        break;
                    case TokenType.PLUS:
                        output += "<PLUS>";
                        break;
                    case TokenType.SLASH:
                        output += "<SLASH>";
                        break;
                    case TokenType.STAR:
                        output += "<STAR>";
                        break;
                    default:
                        System.Environment.Exit(-1);
                        break;
                }

                output += " ";
                output += token.tokenValue;

                Console.WriteLine(output);
            }
        }
    }

    class Program
    {
        private static void Main()
        {
            string source = File.ReadAllText("./source.ss");

            Scanner scanner = new Scanner();
            scanner.ScanLine(source);
            scanner.DisplayTokens();
        }
    }
}
