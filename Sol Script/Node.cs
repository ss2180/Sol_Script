using System;
using System.Collections.Generic;
using System.Text;

namespace Sol_Script
{
    class Node
    {
        public TokenType Type { get; set; }

        public Node(TokenType type)
        {
            Type = type;
        }

        // Builds tree based off stack of prefix tokens.
        public Node(Stack<Token> tokens)
        {
            Type = tokens.Pop().Type;
        }

        

        public Node CreateNode(Token token)
        {
            Node node = null;

            if (token.Type == TokenType.NUMBER)
            {
                node = new NumberNode(token.Type, int.Parse(token.TokenValue));
            }
            else if(token.Type == TokenType.BOOL)
            {
                node = new BoolNode(token.Type, bool.Parse(token.TokenValue));
            }
            else
            {
                node = new Node(token.Type);
            }

            return node;
        }
    }

    class OperatorNode : Node
    {
        public Node Left { get; set; } = null;
        public Node Right { get; set; } = null;

        public OperatorNode(TokenType type) : base(type)
        {

        }

        public void BuildAST(Stack<Token> tokens)
        {
            if (Left == null)
            {
                Left = CreateNode(tokens.Pop());

                if (Left.Type != TokenType.NUMBER && Left.Type != TokenType.BOOL)
                {
                    Left.BuildAST(tokens);
                }
            }

            if (Right == null)
            {
                Right = CreateNode(tokens.Pop());
                if (Right.Type != TokenType.NUMBER && Right.Type != TokenType.BOOL)
                {
                    Right.BuildAST(tokens);
                }
            }
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

    class BoolNode : Node
    {
        public bool Value { get; set; }

        public BoolNode(TokenType type, bool value) : base(type)
        {
            Value = value;
        }
    }
}
