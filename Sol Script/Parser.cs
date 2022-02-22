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

        /// <summary>
        /// Recursively traverses tree to insert node, tries to insert lift side first. Nodes are assumed to be in prefix format for correct ordering.
        /// </summary>
        /// <param name="node">Node to insert</param>
        public bool InsertNode(BaseASTNode node)
        {
            // Consider adding flags nodes to indicate all nodes below flagged node terminate, this should speed up node insertion.

            if (_left == null)
            {
                _left = node;
            }
            else if (_left is BranchableASTNode)
            {
                if ((_left as BranchableASTNode).InsertNode(node) == false)
                {
                    if (_right is BranchableASTNode)
                    {
                        if ((_right as BranchableASTNode).InsertNode(node) == false)
                        {
                            return false;
                        }
                    }
                    else if (_right == null)
                    {
                        _right = node;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if (_right == null)
            {
                _right = node;
            }
            else if (_right is BranchableASTNode)
            {
                if ((_right as BranchableASTNode).InsertNode(node) == false)
                {
                    if (_left is BranchableASTNode)
                    {
                        if ((_left as BranchableASTNode).InsertNode(node) == false)
                        {
                            return false;
                        }
                    }
                    else if (_left == null)
                    {
                        _left = node;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
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

        public BranchableASTNode ConvertExpressionToTree(Token[] tokens)
        {
            int index = 1;

            BranchableASTNode root = new OperatorNode(tokens[0].Type);

            while (index < tokens.Length)
            {
                if(tokens[index].Type == TokenType.NUMBER)
                {
                    root.InsertNode(new NumberNode(int.Parse(tokens[index].TokenValue)));
                }
                else
                {
                    root.InsertNode(new OperatorNode(tokens[index].Type));
                }

                index++;
            }

            return root;
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
