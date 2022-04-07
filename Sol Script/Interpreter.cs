using System;
using System.Collections.Generic;
using System.Text;

namespace Sol_Script
{
    class Interpreter
    {
        public void InterpretExpression(Node expressionRoot)
        {
            if (expressionRoot is Node)
            {
                switch (expressionRoot.Type)
                {
                    case TokenType.PLUS:
                    case TokenType.MINUS:
                    case TokenType.DIVIDE:
                    case TokenType.MULTIPLY:
                    case TokenType.NEGATE:
                        // For now print the result for testing, once more functionality is added, implement print keyword to display output.
                        Console.WriteLine("The result is: {0}", EvaluateNumericExpression(expressionRoot));
                        break;
                    case TokenType.GREATER:
                    case TokenType.LESS:
                    case TokenType.GREATER_OR_EQUAL:
                    case TokenType.LESS_OR_EQUAL:
                    case TokenType.EQUAL:
                    case TokenType.NOTEQUAL:
                        Console.WriteLine("The result is: {0}", EvaluateBooleanExpression(expressionRoot));
                        break;
                    default:
                        throw new Exception($"Unexpected operator {expressionRoot.Type}");
                }
            }
        }

        private float EvaluateNumericExpression(Node expressionRoot)
        {
            if(expressionRoot is NumberNode)
            {
                return (expressionRoot as NumberNode).Value;
            }


            if (expressionRoot is OperatorNode)
            {
                float a = EvaluateNumericExpression((expressionRoot as OperatorNode).Left);
                float b = EvaluateNumericExpression((expressionRoot as OperatorNode).Right);

                switch (expressionRoot.Type)
                {
                    case TokenType.PLUS:
                        return a + b;

                    case TokenType.MINUS:
                        return a - b;

                    case TokenType.MULTIPLY:
                        return a * b;

                    case TokenType.DIVIDE:
                        return a / b;

                    default:
                        throw new Exception("You should not be here!");
                }
            }
            else if(expressionRoot is UnaryNode)
            {
                float a = EvaluateNumericExpression((expressionRoot as UnaryNode).Next);

                switch(expressionRoot.Type)
                {
                    case TokenType.NEGATE:
                        return -a;
                    default:
                        throw new Exception("You should not be here!");
                }
            }
            else
            {
                throw new Exception("Unexpected node type, expected Operator or Unary node!");
            }
        }

        private bool EvaluateBooleanExpression(Node expressionRoot)
        {
            if (expressionRoot is BoolNode)
            {
                return (expressionRoot as BoolNode).Value;
            }

            switch (expressionRoot.Type)
            {
                case TokenType.LESS:
                case TokenType.LESS_OR_EQUAL:
                case TokenType.GREATER:
                case TokenType.GREATER_OR_EQUAL:
                    float a = EvaluateNumericExpression((expressionRoot as OperatorNode).Left);
                    float b = EvaluateNumericExpression((expressionRoot as OperatorNode).Right);

                    switch (expressionRoot.Type)
                    {
                        case TokenType.LESS:
                            return a < b;
                        case TokenType.LESS_OR_EQUAL:
                            return a <= b;
                        case TokenType.GREATER:
                            return a > b;
                        case TokenType.GREATER_OR_EQUAL:
                            return a >= b;
                        default:
                            throw new Exception($"Tree out of order, expected boolean operator for numeric comparisons, recieved {expressionRoot.Type}");
                    }

                case TokenType.EQUAL:
                    return EvaluateBooleanExpression((expressionRoot as OperatorNode).Left) == EvaluateBooleanExpression((expressionRoot as OperatorNode).Right);
                case TokenType.NOTEQUAL:
                    return EvaluateBooleanExpression((expressionRoot as OperatorNode).Left) != EvaluateBooleanExpression((expressionRoot as OperatorNode).Right);

                case TokenType.NOT:
                    return !(EvaluateBooleanExpression((expressionRoot as UnaryNode).Next));
            }

            throw new Exception("You should not be here!");
        }

    }
}
