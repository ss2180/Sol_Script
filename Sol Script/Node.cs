using System;
using System.Collections.Generic;
using System.Text;

namespace Sol_Script
{
    class Node
    {
        public Node Parent { get; } = null;
        public Node Left { get; set; } = null;
        public Node Right { get; set; } = null;

        public TokenType Type { get; set; }

        public Node(TokenType type)
        {
            Type = type;
        }

        public Node(TokenType type, Node parent)
        {
            Type = type;
            Parent = parent;
        }

        // Builds tree based off stack of prefix tokens.
        public Node(Stack<Token> tokens)
        {
            Type = tokens.Pop().Type;
            BuildAST(tokens);
        }

        public void BuildAST(Stack<Token> tokens)
        {
            if (Left == null)
            {
                Left = CreateNode(tokens.Pop());
                if (Left.Type != TokenType.NUMBER)
                {
                    Left.BuildAST(tokens);
                }
            }

            if (Right == null)
            {
                Right = CreateNode(tokens.Pop());
                if (Right.Type != TokenType.NUMBER)
                {
                    Right.BuildAST(tokens);
                }
            }
        }

        private Node CreateNode(Token token)
        {
            Node node = null;

            if (token.Type == TokenType.NUMBER)
            {
                node = new NumberNode(token.Type, this, int.Parse(token.TokenValue));
            }
            else
            {
                node = new Node(token.Type, this);
            }

            return node;
        }
    }

    class NumberNode : Node
    {
        public int Value { get; set; }

        public NumberNode(TokenType type, Node parent, int value) : base(type, parent)
        {
            Value = value;
        }
    }
}
