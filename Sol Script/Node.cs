using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Sol_Script
{
    abstract class Node
    {
        public TokenType Type { get; set; }

        public Scope Scope;

        protected Node(TokenType type)
        {
            Type = type;
        }

        abstract public void BuildAST(Stack<Token> tokens);
        abstract public object Evaluate();

        static public Node Build(Stack<Token> tokens, Scope scope)
        {
            Node node = CreateNode(tokens.Pop());
            node.Scope = scope;

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
                case TokenType.IDENTIFIER:
                    node = new IdentifierNode(token.Type, token.TokenValue);
                    break;
                case TokenType.NOT:
                case TokenType.NEGATE:
                case TokenType.PRINT:
                case TokenType.IF:
                case TokenType.ELIF:
                case TokenType.WHILE:
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
            Next.Scope = Scope;
            Next.BuildAST(tokens);
        }

        public override object Evaluate()
        {
            object a = Next.Evaluate();

            if(Type == TokenType.NEGATE)
            {
                if(a is int intval)
                {
                    return -intval;
                }
                else if(a is float floatval)
                {
                    return -floatval;
                }

                throw new Exception($"Can not negate type: {a.GetType()}");
            }
            else if(Type == TokenType.NOT)
            {
                if(a is bool boolVal)
                {
                    return !boolVal;
                }

                throw new Exception($"Expected bool value, received {a.GetType()}");
            }
            else if(Type == TokenType.PRINT)
            {
                Console.WriteLine(a);

                return 0;
            }
            else if(Type == TokenType.IF || Type == TokenType.WHILE || Type == TokenType.ELIF)
            {
                if(a is bool boolVal)
                {
                    return boolVal;
                }
                throw new Exception($"Expected bool value, received {a.GetType()}");
            }
            else
            {
                throw new Exception($"Type is not a unary type: {Type}");
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
            return Regex.Unescape(Value);
        }
    }

    class IdentifierNode : Node
    {
        public string Name { get; set; }

        public IdentifierNode(TokenType type, string name) : base(type)
        {
            Name = name;
        }

        public override void BuildAST(Stack<Token> tokens)
        {
            return;
        }

        public override object Evaluate()
        {
            object result;
            // Loop over dictionarys starting at the local scope and working outwards.
            for(int i = Scope.variables.Count - 1; i >= 0; i--)
            {
                if (Scope.variables[i].TryGetValue(Name, out result))
                {
                    return result;
                }
            }

            throw new Exception($"Variable of name '{Name}' is not defined in this scope.");
        }
    }
}
