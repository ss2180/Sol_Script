using System;
using System.Collections.Generic;
using System.Text;

namespace Sol_Script
{
    class Parser
    {
        private Stack<Token> OutputStack = new Stack<Token>();
        private Stack<Token> OperatorStack = new Stack<Token>();

        public Stack<Token> Parse(List<Token> tokens)
        {
            Token token = tokens[0];
            List<Token> expression = new List<Token>();
            
            switch(token.Type)
            {
                case TokenType.IDENTIFIER:
                    if(tokens[1].Type == TokenType.ASSIGN)
                    {
                        expression = tokens.GetRange(2, tokens.Count - 2);
                        Stack<Token> tokenStack = ConvertToPrefix(expression);

                        // Push assignment and identifer back onto stack.
                        tokenStack.Push(tokens[0]);
                        tokenStack.Push(tokens[1]);

                        return tokenStack;
                    }

                    throw new Exception("Expected assignment after identifier");

                case TokenType.PRINT:
                    if(tokens[1].Type == TokenType.LEFT_BRACKET)
                    {
                        Token currentToken = tokens[2];
                        int leftBracketCount = 0;

                        for(int i = 3; currentToken.Type != TokenType.RIGHT_BRACKET || leftBracketCount != 0; i++)
                        {
                            if(i > tokens.Count)
                            {
                                throw new Exception("Missing right parenthesis");
                            }
                            if(currentToken.Type == TokenType.LEFT_BRACKET)
                            {
                                leftBracketCount++;
                            }
                            else if(currentToken.Type == TokenType.RIGHT_BRACKET)
                            {
                                leftBracketCount--;
                            }

                            expression.Add(currentToken);
                            currentToken = tokens[i];
                        }

                        if (expression.Count != 0)
                        {
                            Stack<Token> tokenStack = ConvertToPrefix(expression);
                            tokenStack.Push(token);

                            return tokenStack;
                        }

                        throw new Exception("Missing expression for print statement");
                    }
                    throw new Exception("Expected parenthesis after print statement");

                default:
                    throw new Exception($"Unexpected token: {token.Type}");
            }
        }

        private Stack<Token> ConvertToPrefix(List<Token> tokens)
        {
            tokens.Reverse();

            foreach (Token token in tokens)
            {
                switch (token.Type)
                {
                    case TokenType.INT_NUM:
                    case TokenType.FLOAT_NUM:
                    case TokenType.BOOL:
                    case TokenType.STRING:
                    case TokenType.IDENTIFIER:
                        OutputStack.Push(token);
                        break;

                    case TokenType.PLUS:
                    case TokenType.MINUS:
                    case TokenType.DIVIDE:
                    case TokenType.MULTIPLY:
                    case TokenType.GREATER:
                    case TokenType.LESS:
                    case TokenType.GREATER_OR_EQUAL:
                    case TokenType.LESS_OR_EQUAL:
                    case TokenType.EQUAL:
                    case TokenType.NOTEQUAL:
                    case TokenType.NOT:
                    case TokenType.NEGATE:
                        HandleOperator(token);
                        break;

                    case TokenType.LEFT_BRACKET:
                        token.Type = TokenType.RIGHT_BRACKET;
                        token.TokenValue = ")";
                        HandleClosedBracket(token);
                        break;

                    case TokenType.RIGHT_BRACKET:
                        token.Type = TokenType.LEFT_BRACKET;
                        token.TokenValue = "(";
                        OperatorStack.Push(token);
                        break;

                    default:
                        Console.WriteLine("Unexpected token: {0}", token.Type);
                        break;
                }
            }

            while (OperatorStack.Count != 0)
            {
                Token operand = OperatorStack.Pop();
                OutputStack.Push(operand);
            }

            return OutputStack;
        }

        private void HandleOperator(Token token)
        {
            if (OperatorStack.Count > 0)
            {
                TokenType operatorTop = OperatorStack.Peek().Type;

                while (IsPrecidenceLowerThanStack(token.Type) && operatorTop != TokenType.LEFT_BRACKET)
                {
                    if (operatorTop == TokenType.LEFT_BRACKET)
                    {
                        break;
                    }

                    Token operand = OperatorStack.Pop();
                    OutputStack.Push(operand);

                    if (OperatorStack.Count == 0)
                    {
                        break;
                    }

                    operatorTop = OperatorStack.Peek().Type;
                }
            }

            OperatorStack.Push(token);
        }

        private void HandleClosedBracket(Token token)
        {
            TokenType operatorTop = OperatorStack.Peek().Type;

            while (operatorTop != TokenType.LEFT_BRACKET)
            {
                Token operand = OperatorStack.Pop();
                OutputStack.Push(operand);

                operatorTop = OperatorStack.Peek().Type;
            }

            OperatorStack.Pop();
        }

        private bool IsPrecidenceLowerThanStack(TokenType op)
        {
            TokenType operatorTop = OperatorStack.Peek().Type;

            int stackPrecidenceLevel = GetOperatorPrecidence(operatorTop);
            int operatorPrecidenceLevel = GetOperatorPrecidence(op);

            if (stackPrecidenceLevel > operatorPrecidenceLevel)
            {
                return true;
            }

            return false;
        }

        private int GetOperatorPrecidence(TokenType op)
        {
            switch (op)
            {
                case TokenType.LEFT_BRACKET:
                    return 5;
                case TokenType.NOT:
                case TokenType.NEGATE:
                    return 4;
                case TokenType.DIVIDE:
                case TokenType.MULTIPLY:
                    return 3;
                case TokenType.MINUS:
                case TokenType.PLUS:
                    return 2;
                case TokenType.GREATER:
                case TokenType.LESS:
                case TokenType.GREATER_OR_EQUAL:
                case TokenType.LESS_OR_EQUAL:
                    return 1;
                case TokenType.EQUAL:
                case TokenType.NOTEQUAL:
                    return 0;

                default:
                    throw new Exception($"Unexpected token type: {op}");

            }
        }
    }
}
