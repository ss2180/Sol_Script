﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Sol_Script
{
    class Node
    {
        private Node _left = null;
        private Node _right = null;

        public TokenType Type { get; set; }

        public Node(TokenType type)
        {
            Type = type;
        }

        public void BuildAST(Stack<Token> tokens)
        {
            if(_left == null)
            {
                _left = CreateNode(tokens.Pop());
                if(_left.Type != TokenType.NUMBER)
                {
                    _left.BuildAST(tokens);
                }
            }

            if(_right == null)
            {
                _right = CreateNode(tokens.Pop());
                if(_right.Type != TokenType.NUMBER)
                {
                    _right.BuildAST(tokens);
                }
            }
        }

        private Node CreateNode(Token token)
        {
            Node node = null;

            if(token.Type == TokenType.NUMBER)
            {
                node = new NumberNode(token.Type, int.Parse(token.TokenValue));
            }
            else
            {
                node = new Node(token.Type);
            }

            return node;
        }
    }

    class NumberNode : Node
    {
        public int Value { get; set; }

        public NumberNode(TokenType type, int value) : base(type)
        {
            Value = value;
        }
    }

    class Parser
    {
        private Stack<Token> OutputStack = new Stack<Token>();
        private Stack<Token> OperatorStack = new Stack<Token>();

        public Token[] ConvertToPrefix(List<Token> tokens)
        {
            tokens.Reverse();

            foreach (Token token in tokens)
            {
                switch (token.Type)
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
                        break;
                }
            }

            while (OperatorStack.Count != 0)
            {
                Token operand = OperatorStack.Pop();
                OutputStack.Push(operand);
            }

            int stackLength = OutputStack.Count;
            Token[] outputArray = new Token[stackLength];

            while(stackLength != 0)
            {
                outputArray[stackLength - 1] = OutputStack.Pop();

                stackLength = OutputStack.Count;
            }

            Array.Reverse(outputArray);

            return outputArray;
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

        private bool IsPrecidenceLowerThanStack(TokenType operator1)
        {
            TokenType operatorTop = OperatorStack.Peek().Type;

            int stackPrecidenceLevel = 0;
            int operatorPrecidenceLevel = 0;

            switch (operatorTop)
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

            if (stackPrecidenceLevel > operatorPrecidenceLevel)
            {
                return true;
            }

            return false;
        }
    }
}
