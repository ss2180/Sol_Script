using System;
using System.Collections.Generic;
using System.IO;

namespace Sol_Script
{
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
                switch (line[index])
                {
                    case ' ':
                        index++;
                        break;
                    case '-':
                        _tokens.Add(new Token(TokenType.MINUS, "-"));
                        index++;
                        break;
                    case '+':
                        _tokens.Add(new Token(TokenType.PLUS, "+"));
                        index++;
                        break;
                    case '*':
                        _tokens.Add(new Token(TokenType.MULTIPLY, "*"));
                        index++;
                        break;
                    case '/':
                        _tokens.Add(new Token(TokenType.DIVIDE, "/"));
                        index++;
                        break;
                    case '(':
                        _tokens.Add(new Token(TokenType.LEFT_BRACKET, "("));
                        index++;
                        break;
                    case ')':
                        _tokens.Add(new Token(TokenType.RIGHT_BRACKET, ")"));
                        index++;
                        break;
                    case '!':
                        if (line[index + 1] == '=')
                        {
                            _tokens.Add(new Token(TokenType.NOTEQUAL, "!="));
                            index = index + 2;
                        }
                        else
                        {
                            _tokens.Add(new Token(TokenType.NOT, "!"));
                            index++;
                        }
                        break;
                    case '=':
                        if (line[index + 1] == '=') //TODO: Make sure index is in bounds somehow.
                        {
                            _tokens.Add(new Token(TokenType.EQUAL, "=="));
                            index = index + 2;
                        }
                        else
                        {
                            _tokens.Add(new Token(TokenType.ASSIGN, "="));
                            index++;
                        }
                        break;
                    case '>':
                        if (line[index + 1] == '=')
                        {
                            _tokens.Add(new Token(TokenType.GREATER_OR_EQUAL, ">="));
                            index = index + 2;
                        }
                        else
                        {
                            _tokens.Add(new Token(TokenType.GREATER, ">"));
                            index++;
                        }
                        break;
                    case '<':
                        if (line[index + 1] == '=')
                        {
                            _tokens.Add(new Token(TokenType.LESS_OR_EQUAL, "<="));
                            index = index + 2;
                        }
                        else
                        {
                            _tokens.Add(new Token(TokenType.LESS, "<"));
                            index++;
                        }
                        break;
                    default:
                        if (char.IsDigit(line[index]))
                        {
                            index = ScanNumber(line, index);
                        }
                        else if(char.IsLetter(line[index]))
                        {
                            index = ScanWord(line, index);
                        }
                        else
                        {
                            throw new IOException($"Character '{line[index]}' is not a recognised token pattern.");
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Scans for the next token starting at the provided index and adds the token to the token list.
        /// </summary>
        /// <returns>The index of the start of the next token.</returns>
        private int ScanNumber(string text, int index)
        {
            int nextIndex = index + 1;
            int numberCharLength = 1;

            if (nextIndex < text.Length)
            {
                while (char.IsDigit(text[nextIndex]))
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

        private int ScanWord(string text, int index)
        {
            int EOF = text.Length;

            switch (text[index])
            {
                //true
                case 't':
                    if(index + 3 < EOF)
                    {
                        if(text.Substring(index, 4).ToLower() == "true")
                        {
                            _tokens.Add(new Token(TokenType.BOOL, "true"));
                            index += 4;
                        }
                        else
                        {
                            index = ScanIdentifier(text, index);
                        }
                    }
                    break;

                //false
                case 'f':
                    if (index + 4 < EOF)
                    {
                        if (text.Substring(index, 5).ToLower() == "false")
                        {
                            _tokens.Add(new Token(TokenType.BOOL, "false"));
                            index += 5;
                        }
                        else
                        {
                            index = ScanIdentifier(text, index);
                        }
                    }
                    break;
                default:
                    index = ScanIdentifier(text, index);
                    break;
            }

            return index;
        }

        private int ScanIdentifier(string text, int index)
        {
            int nextIndex = index + 1;
            int tokenLength = 1;

            if (nextIndex < text.Length)
            {
                while (char.IsLetterOrDigit(text[nextIndex]))
                {
                    nextIndex++;
                    tokenLength++;

                    if (nextIndex >= text.Length)
                    {
                        break;
                    }
                }
            }

            string identiferString = text.Substring(index, tokenLength);
            _tokens.Add(new Token(TokenType.IDENTIFIER, identiferString));

            return nextIndex;
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
