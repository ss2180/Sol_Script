using System;
using System.Collections.Generic;
using System.Text;

namespace Sol_Script
{
    class Parser
    {
        private Stack<Token> OutputStack = new Stack<Token>();
        private Stack<Token> OperatorStack = new Stack<Token>();
        private TokenType? LastTokenType = null;

        public Stack<Token> ConvertToPrefix(List<Token> tokens)
        {
            tokens.Reverse();

            foreach (Token token in tokens)
            {
                switch (token.Type)
                {
                    case TokenType.NUMBER:
                    case TokenType.BOOL:
                        OutputStack.Push(token);
                        LastTokenType = token.Type;
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
                        HandleOperator(token);
                        LastTokenType = token.Type;
                        break;

                    case TokenType.LEFT_BRACKET:
                        token.Type = TokenType.RIGHT_BRACKET;
                        token.TokenValue = ")";
                        HandleClosedBracket(token);
                        LastTokenType = token.Type;
                        break;

                    case TokenType.RIGHT_BRACKET:
                        token.Type = TokenType.LEFT_BRACKET;
                        token.TokenValue = "(";
                        OperatorStack.Push(token);
                        LastTokenType = token.Type;
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
            // Check if '-' is a negation operator
            if(LastTokenType != TokenType.NUMBER && LastTokenType != TokenType.RIGHT_BRACKET && token.Type == TokenType.MINUS)
            {
                // Chage operator type to negation to be passed into shunting yard.
                token.Type = TokenType.NEGATE;
            }

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
