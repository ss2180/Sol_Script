using System;
using System.Collections.Generic;
using System.IO;

namespace Sol_Script
{
    enum Instructions
    {
        ADD, SUBTRACT, DIVIDE, MULTIPLY
    }

    class Parser
    {
        private Stack<Token> OutputStack = new Stack<Token>();
        private Stack<Token> OperatorStack = new Stack<Token>();

        public Stack<Token> ParseExpression(List<Token> tokens)
        {
            foreach(Token token in tokens)
            {
                switch(token.Type)
                {
                    case TokenType.NUMBER:
                        OutputStack.Push(token);
                        break;
                    case TokenType.PLUS:
                        HandleOperator(token);
                        break;
                    case TokenType.MINUS:
                        HandleOperator(token);
                        break;
                    case TokenType.SLASH:
                        HandleOperator(token);
                        break;
                    case TokenType.STAR:
                        HandleOperator(token);
                        break;
                    case TokenType.LEFT_BRACKET:
                        OperatorStack.Push(token);
                        break;
                    case TokenType.RIGHT_BRACKET:
                        HandleClosedBracket(token);
                        break;
                    default:
                        break;
                }
            }

            while(OperatorStack.Count != 0)
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

                while (IsPrecidenceLowerThanStack(token.Type))
                {
                    if(operatorTop == TokenType.LEFT_BRACKET)
                    {
                        break;
                    }

                    Token operand = OperatorStack.Pop();
                    OutputStack.Push(operand);

                    if(OperatorStack.Count == 0)
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

            while(operatorTop != TokenType.LEFT_BRACKET)
            {
                Token operand = OperatorStack.Pop();
                OutputStack.Push(operand);

                operatorTop = OperatorStack.Peek().Type;
            }

            OperatorStack.Pop();
        }

        private bool IsPrecidenceLowerThanStack(TokenType operator1)
        {
            TokenType operatorTop = OperatorStack.Peek().Type;

            int stackPrecidenceLevel = 0;
            int operatorPrecidenceLevel = 0;

            switch(operatorTop)
            {
                case TokenType.SLASH:
                    stackPrecidenceLevel = 3;
                    break;
                case TokenType.STAR:
                    stackPrecidenceLevel = 3;
                    break;
                case TokenType.MINUS:
                    stackPrecidenceLevel = 2;
                    break;
                case TokenType.PLUS:
                    stackPrecidenceLevel = 2;
                    break;
            }

            switch (operator1)
            {
                case TokenType.SLASH:
                    operatorPrecidenceLevel = 3;
                    break;
                case TokenType.STAR:
                    operatorPrecidenceLevel = 3;
                    break;
                case TokenType.MINUS:
                    operatorPrecidenceLevel = 2;
                    break;
                case TokenType.PLUS:
                    operatorPrecidenceLevel = 2;
                    break;
            }

            if(stackPrecidenceLevel >= operatorPrecidenceLevel)
            {
                return true;
            }

            return false;
        }
    }

    class Program
    {
        private static int Main()
        {
            string source = File.ReadAllText("./source.sol");

            Scanner scanner = new Scanner();

            try
            {
                scanner.ScanLine(source);
            }
            catch(IOException e)
            {
                Console.WriteLine(e);

                return -1;
            }

            List<Token> tokens = scanner.Tokens;

            Parser parser = new Parser();

            Stack<Token> expression = parser.ParseExpression(tokens);

            return 0;
        }
    }
}
