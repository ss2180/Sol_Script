using System;
using System.Collections.Generic;
using System.Text;

namespace Sol_Script
{
    enum Instruction
    {
        ADD, SUBTRACT, DIVIDE, MULTIPLY,

        UNINITIALISED
    }

    abstract class BaseASTNode
    {
        public BaseASTNode()
        {
        }
    }

    abstract class BranchableASTNode : BaseASTNode
    {
        public  BaseASTNode _left { get; set; }
        public  BaseASTNode _right { get; set; }

        public BranchableASTNode()
        {
            _left = null;
            _right = null;
        }

        public BranchableASTNode(BaseASTNode left, BaseASTNode right)
        {
            _left = left;
            _right = right;
        }
    }

    class OperatorNode: BranchableASTNode
    {
        public TokenType operatorType { get; set; }

        public OperatorNode(TokenType instruction) : base()
        {
            operatorType = instruction;
        }

        public OperatorNode(TokenType instruction, BaseASTNode left, BaseASTNode right) : base(left, right)
        {
            operatorType = instruction;
        }
    }

    class NumberNode : BaseASTNode
    {
        public int Value { get; set; }

        public NumberNode() : base()
        {
            Value = 0;
        }

        public NumberNode(int value) : base()
        {
            Value = value;
        }
    }


    class Parser
    {
        private Stack<Token> OutputStack = new Stack<Token>();
        private Stack<Token> OperatorStack = new Stack<Token>();

        
        public Token[] ParseExpression(List<Token> tokens)
        {
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
                        OperatorStack.Push(token);
                        break;
                    case TokenType.RIGHT_BRACKET:
                        HandleClosedBracket(token);
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

            return outputArray;
        }

        public BranchableASTNode ConvertExpressionToTree(Token[] tokens)
        {
            Stack<int> numbers = new Stack<int>();

            Stack<BranchableASTNode> nodes = new Stack<BranchableASTNode>();

            foreach( Token token in tokens)
            {
                if(token.Type == TokenType.NUMBER)
                {
                    numbers.Push(int.Parse(token.TokenValue));
                }
                else
                {
                    // Create subtree if current token precidence is higher than top of node stack.

                    if(numbers.Count >= 2)
                    {
                        OperatorNode operatorNode = new OperatorNode(token.Type, new NumberNode(numbers.Pop()), new NumberNode(numbers.Pop()));

                        nodes.Push(operatorNode);
                    }
                    else if(nodes.Count >= 2)
                    {
                        OperatorNode node1 = nodes.Pop() as OperatorNode;
                        OperatorNode node2 = nodes.Pop() as OperatorNode;

                        nodes.Push(new OperatorNode(token.Type, node2, node1));
                    }
                    else
                    {
                        OperatorNode node = nodes.Pop() as OperatorNode;

                        OperatorNode operatorNode = new OperatorNode(token.Type, node, new NumberNode(numbers.Pop()));

                        nodes.Push(operatorNode);
                    }
                }
            }

            return nodes.Pop();
        }

        private void HandleOperator(Token token)
        {
            if (OperatorStack.Count > 0)
            {
                TokenType operatorTop = OperatorStack.Peek().Type;

                while (IsPrecidenceLowerThanStack(token.Type))
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

            if (stackPrecidenceLevel >= operatorPrecidenceLevel)
            {
                return true;
            }

            return false;
        }
    }
}
