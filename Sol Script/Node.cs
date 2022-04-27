using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sol_Script
{
    abstract class Node
    {
        public TokenType Type { get; set; }

        protected Node(TokenType type)
        {
            Type = type;
        }

        abstract public void BuildAST(Stack<Token> tokens);
        abstract public object Evaluate();

        static public Node Build(Stack<Token> tokens)
        {
            Node node = CreateNode(tokens.Pop());

            node.BuildAST(tokens);

            return node;
        }

        static protected Node CreateNode(Token token)
        {
            Node node = null;

            switch(token.Type)
            {
                case TokenType.INT_NUM:
                    node = new IntNumNode(token.Type, int.Parse(token.TokenValue));
                    break;
                case TokenType.FLOAT_NUM:
                    node = new FloatNumNode(token.Type, float.Parse(token.TokenValue));
                    break;
                case TokenType.BOOL:
                    node = new BoolNode(token.Type, bool.Parse(token.TokenValue));
                    break;
                case TokenType.STRING:
                    node = new StringNode(token.Type, token.TokenValue);
                    break;
                case TokenType.NOT:
                case TokenType.NEGATE:
                    node = new UnaryNode(token.Type);
                    break;
                default:
                    node = new OperatorNode(token.Type);
                    break;
            }

            return node;
        }
    }

    class UnaryNode : Node
    {
        public Node Next { get; set; } = null;

        public UnaryNode(TokenType type) : base(type)
        {

        }

        public override void BuildAST(Stack<Token> tokens)
        {
            Next = CreateNode(tokens.Pop());
            Next.BuildAST(tokens);
        }

        public override object Evaluate()
        {
            object a = Next.Evaluate();

            if(a is int intVal)
            {
                if (Type == TokenType.NEGATE)
                {
                    return -intVal;
                }

                throw new Exception("Expected NEGATE operator");
            }
            else if(a is float floatVal)
            {
                if (Type == TokenType.NEGATE)
                {
                    return -floatVal;
                }

                throw new Exception("Expected NEGATE operator");
            }
            else if(a is bool boolVal)
            {
                if(Type == TokenType.NOT)
                {
                    return !boolVal;
                }

                throw new Exception("Expected NOT operator");
            }
            else
            {
                throw new Exception($"Unexpected Type: {Type}");
            }
        }
    }

    

    class IntNumNode : Node
    {
        public int Value { get; set; }

        public IntNumNode(TokenType type, int value) : base(type)
        {
            Value = value;
        }

        public override void BuildAST(Stack<Token> tokens)
        {
            return;
        }

        public override object Evaluate()
        {
            return Value;
        }
    }

    class FloatNumNode : Node
    {
        public float Value { get; set; }

        public FloatNumNode(TokenType type, float value) : base(type)
        {
            Value = value;
        }

        public override void BuildAST(Stack<Token> tokens)
        {
            return;
        }

        public override object Evaluate()
        {
            return Value;
        }
    }

    class BoolNode : Node
    {
        public bool Value { get; set; }

        public BoolNode(TokenType type, bool value) : base(type)
        {
            Value = value;
        }

        public override void BuildAST(Stack<Token> tokens)
        {
            return;
        }

        public override object Evaluate()
        {
            return Value;
        }
    }

    class StringNode : Node
    {
        public string Value { get; set; }

        public StringNode(TokenType type, string value) : base(type)
        {
            Value = value;
        }

        public override void BuildAST(Stack<Token> tokens)
        {
            return;
        }

        public override object Evaluate()
        {
            return Value;
        }
    }
}
