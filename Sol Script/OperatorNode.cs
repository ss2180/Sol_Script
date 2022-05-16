using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sol_Script
{
    class OperatorNode : Node
    {
        public Node Left { get; set; } = null;
        public Node Right { get; set; } = null;

        public OperatorNode(TokenType type) : base(type)
        {

        }

        public override void BuildAST(Stack<Token> tokens)
        {
            Left = CreateNode(tokens.Pop());
            Left.Scope = Scope;
            Left.BuildAST(tokens);

            Right = CreateNode(tokens.Pop());
            Right.Scope = Scope;
            Right.BuildAST(tokens);

        }

        public override object Evaluate()
        {
            switch (Type)
            {
                case TokenType.PLUS:
                case TokenType.MINUS:
                case TokenType.DIVIDE:
                case TokenType.MULTIPLY:
                    return EvaluateMathematicalOperator();
                case TokenType.GREATER:
                case TokenType.LESS:
                case TokenType.GREATER_OR_EQUAL:
                case TokenType.LESS_OR_EQUAL:
                case TokenType.EQUAL:
                case TokenType.NOTEQUAL:
                    return EvaluateBooleanOperator();
                case TokenType.ASSIGN:
                    return HandleAssignment();
                default:
                    throw new Exception($"Unexpected operator {Type}");
            }
        }

        private object HandleAssignment()
        {
            object expression = Right.Evaluate();

            if(Left is IdentifierNode inode)
            {
                if (Scope.variables.TryGetValue(inode.Name, out _))
                {
                    Scope.variables[inode.Name] = expression;
                }
                else
                {
                    Scope.variables.Add(inode.Name, expression);
                }

                return 0;
            }
            throw new Exception("Missing identifier for assignment");
        }

        private object EvaluateMathematicalOperator()
        {
            object a = Left.Evaluate();
            object b = Right.Evaluate();

            if (a is int)
            {
                if (b is int)
                {
                    return HandleIntArithmetic(a, b);
                }
                else if (b is float)
                {
                    return HandleFloatArithmetic(a, b);
                }

                throw new Exception($"Expected right operand to be either int or float, recieved {b.GetType()}");
            }
            else if (a is float)
            {
                if (b is int || b is float)
                {
                    return HandleFloatArithmetic(a, b);
                }

                throw new Exception($"Expected right operand to be either int or float, recieved {b.GetType()}");
            }
            else if( a is string)
            {
                if(b is string)
                {
                    if (Type == TokenType.PLUS)
                    {
                        string val1 = (string)a;
                        string val2 = (string)b;

                        return val1 + val2;
                    }

                    throw new Exception($"Unexpected operator on string {Type}, did you mean to concatenate?");
                }

                throw new Exception($"Expected right operand to be a string, recieved {b.GetType()}");
            }
            else
            {
                throw new Exception($"Unexpected types for {Type} node, Left:{a.GetType()} Right{b.GetType()}");
            }
        }

        private object HandleFloatArithmetic(object a, object b)
        {
            float val1 = (float)Convert.ChangeType(a, TypeCode.Single);
            float val2 = (float)Convert.ChangeType(b, TypeCode.Single);

            switch (Type)
            {
                case TokenType.PLUS:
                    return val1 + val2;

                case TokenType.MINUS:
                    return val1 - val2;

                case TokenType.MULTIPLY:
                    return val1 * val2;

                case TokenType.DIVIDE:
                    return val1 / val2;

                default:
                    throw new Exception("You should not be here!");
            }
        }

        private object HandleIntArithmetic(object a, object b)
        {
            int val1 = (int)a;
            int val2 = (int)b;

            switch (Type)
            {
                case TokenType.PLUS:
                    return val1 + val2;

                case TokenType.MINUS:
                    return val1 - val2;

                case TokenType.MULTIPLY:
                    return val1 * val2;

                case TokenType.DIVIDE:
                    return val1 / val2;

                default:
                    throw new Exception("You should not be here!");
            }
        }

        private object EvaluateBooleanOperator()
        {
            switch (Type)
            {
                case TokenType.LESS:
                case TokenType.LESS_OR_EQUAL:
                case TokenType.GREATER:
                case TokenType.GREATER_OR_EQUAL:
                    object a = Left.Evaluate();
                    object b = Right.Evaluate();

                    if (a is int)
                    {
                        if(b is int)
                        {
                            return HandleIntComparison(a, b);
                        }
                        else if(b is float)
                        {
                            return HandleFloatComparison(a, b);
                        }

                        throw new Exception($"Expected right operand to be either int or float, recieved {b.GetType()}");
                    }
                    else if (a is float)
                    {
                        if(b is int || b is float)
                        {
                            return HandleFloatComparison(a, b);
                        }

                        throw new Exception($"Expected right operand to be either int or float, recieved {b.GetType()}");
                    }
                    else
                    {
                        throw new Exception($"Unexpected types for {Type} node, Left:{a.GetType()} Right{b.GetType()}");
                    }

                case TokenType.EQUAL:
                case TokenType.NOTEQUAL:
                    return HandleEquality(Left.Evaluate(), Right.Evaluate(), Type);
                default:
                    throw new Exception($"Unexpected token type {Type}");
            }
        }

        private object HandleIntComparison(object a, object b)
        {
            int val1 = (int)a;
            int val2 = (int)b;

            switch (Type)
            {
                case TokenType.LESS:
                    return val1 < val2;
                case TokenType.LESS_OR_EQUAL:
                    return val1 <= val2;
                case TokenType.GREATER:
                    return val1 > val2;
                case TokenType.GREATER_OR_EQUAL:
                    return val1 >= val2;
                default:
                    throw new Exception($"Tree out of order, expected boolean operator for numeric comparisons, recieved {Type}");
            }
        }

        private object HandleFloatComparison(object a, object b)
        {
            float val1 = (float)Convert.ChangeType(a, TypeCode.Single);
            float val2 = (float)Convert.ChangeType(b, TypeCode.Single);

            switch (Type)
            {
                case TokenType.LESS:
                    return val1 < val2;
                case TokenType.LESS_OR_EQUAL:
                    return val1 <= val2;
                case TokenType.GREATER:
                    return val1 > val2;
                case TokenType.GREATER_OR_EQUAL:
                    return val1 >= val2;
                default:
                    throw new Exception($"Tree out of order, expected boolean operator for numeric comparisons, recieved {Type}");
            }
        }

        private object HandleEquality(object a, object b, TokenType type)
        {
            if(a is bool)
            {
                if(b is bool)
                {
                    if(type == TokenType.EQUAL)
                    {
                        return (bool)a == (bool)b;
                    }
                    else if(type == TokenType.NOTEQUAL)
                    {
                        return (bool)a != (bool)b;
                    }

                    throw new Exception($"Expected operator type to be EQUALITY or INEQUALITY, recieved {type}");
                }

                throw new Exception($"Expected right operand to be boolean, recieved {b.GetType()}");
            }
            else if(a is int)
            {
                if(b is int)
                {
                    int val1 = (int)a;
                    int val2 = (int)b;

                    if (type == TokenType.EQUAL)
                    {
                        return val1 == val2;
                    }
                    else if (type == TokenType.NOTEQUAL)
                    {
                        return val1 != val2;
                    }

                    throw new Exception($"Expected operator type to be EQUALITY or INEQUALITY, recieved {type}");
                }
                else if(b is float)
                {
                    float val1 = (float)Convert.ChangeType(a, TypeCode.Single);
                    float val2 = (float)Convert.ChangeType(b, TypeCode.Single);

                    if (type == TokenType.EQUAL)
                    {
                        return val1 == val2;
                    }
                    else if (type == TokenType.NOTEQUAL)
                    {
                        return val1 != val2;
                    }

                    throw new Exception($"Expected operator type to be EQUALITY or INEQUALITY, recieved {type}");
                }

                throw new Exception($"Expected right operand to be either int or float, recieved {b.GetType()}");
            }
            else if(a is float)
            {
                if (b is float || b is int)
                {
                    float val1 = (float)Convert.ChangeType(a, TypeCode.Single);
                    float val2 = (float)Convert.ChangeType(b, TypeCode.Single);

                    if (type == TokenType.EQUAL)
                    {
                        return val1 == val2;
                    }
                    else if (type == TokenType.NOTEQUAL)
                    {
                        return val1 != val2;
                    }

                    throw new Exception($"Expected operator type to be EQUALITY or INEQUALITY, recieved {type}");
                }

                throw new Exception($"Expected right operand to be either int or float, recieved {b.GetType()}");
            }
            else if(a is string)
            {
                if(b is string)
                {
                    string val1 = (string)a;
                    string val2 = (string)b;

                    if (type == TokenType.EQUAL)
                    {
                        return val1 == val2;
                    }
                    else if (type == TokenType.NOTEQUAL)
                    {
                        return val1 != val2;
                    }

                    throw new Exception($"Expected operator type to be EQUALITY or INEQUALITY, recieved {type}");
                }

                throw new Exception($"Expected right operand to be a string, recieved {b.GetType()}");
            }

            throw new Exception($"Unexpected types for {Type} node, Left:{a.GetType()} Right{b.GetType()}");
        }
    }
}
