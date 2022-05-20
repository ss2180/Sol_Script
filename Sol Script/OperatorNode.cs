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
                case TokenType.AND:
                case TokenType.OR:
                    return EvaluateBooleanOperator();
                case TokenType.ASSIGN:
                    return HandleAssignment();
                case TokenType.INPUT:
                    return HandleInput();
                case TokenType.LISTADD:
                    return HandleListAdd();
                case TokenType.LISTREMOVE:
                    return HandleListRemove();
                case TokenType.LISTGET:
                    return HandleListGet();
                default:
                    throw new Exception($"Unexpected operator {Type}");
            }
        }

        private object HandleListGet()
        {
            object _list = Left.Evaluate();
            object index = Right.Evaluate();

            if (_list is List<object> list)
            {
                if (index is int index_val)
                {
                    if (index_val > list.Count - 1 || index_val < 0)
                    {
                        throw new Exception("index must be within the bounds of the list");
                    }

                    return list[index_val];
                }
                throw new Exception("listget index must be an integer value");

            }
            throw new Exception("A listget must be passed into listremove");
        }

        private object HandleListRemove()
        {
            object _list = Left.Evaluate();
            object index = Right.Evaluate();

            if (_list is List<object> list)
            {
                if(index is int index_val)
                {
                    if(index_val > list.Count - 1 || index_val < 0)
                    {
                        throw new Exception("index must be within the bounds of the list");
                    }

                    list.RemoveAt(index_val);

                    return 0;
                }
                throw new Exception("listremove index must be an integer value");
                
            }
            throw new Exception("A list must be passed into listremove");
        }

        private object HandleListAdd()
        {
            object _list = Left.Evaluate();
            object value = Right.Evaluate();

            if(_list is List<object> list)
            {
                if(list.Count > 0)
                {
                    Console.WriteLine(list[0].GetType());
                    Console.WriteLine(value.GetType());
                    if (list[0].GetType() != value.GetType())
                    {
                        throw new Exception($"Cannot assign {value.GetType()} to list of {list[0].GetType()}");
                    }
                }

                list.Add(value);

                return 0;
            }
            throw new Exception("A list must be passed into listadd");
        }

        private object HandleInput()
        {
            object text = Left.Evaluate();
            object parseFlag = Right.Evaluate();

            if (text is string stringVal)
            {
                if(parseFlag is int pFlag)
                {
                    Console.Write(stringVal);
                    string userInput = Console.ReadLine();
                    switch (pFlag)
                    {
                        case 0:
                            return userInput;
                        case 1:
                            int iResult;
                            if(int.TryParse(userInput, out iResult))
                            {
                                return iResult;
                            }
                            throw new Exception($"Could not parse '{userInput}' to an int");
                        case 2:
                            float fResult;
                            if (float.TryParse(userInput, out fResult))
                            {
                                return fResult;
                            }
                            throw new Exception($"Could not parse '{userInput}' to a float.");
                        case 3:
                            bool bResult;
                            if (bool.TryParse(userInput, out bResult))
                            {
                                return bResult;
                            }
                            throw new Exception($"Could not parse '{userInput}' to a bool");
                        default:
                            throw new Exception($"Invalde flag '{pFlag}'");
                    }
                }
                throw new Exception("Expected parse flag to be an integer.");
                
            }
            throw new Exception($"Input expects string value, received {text.GetType()}");
        }

        private object HandleAssignment()
        {
            object expression = Right.Evaluate();

            if(Left is IdentifierNode inode)
            {
                bool assigned = false;
                foreach(var dict in Scope.variables)
                {
                    if(dict.TryGetValue(inode.Name, out _))
                    {
                        dict[inode.Name] = expression;
                        assigned = true;
                    }
                }

                if(!assigned)
                {
                    Scope.variables[Scope.variables.Count - 1].Add(inode.Name, expression);
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
                case TokenType.AND:
                case TokenType.OR:
                    return HandleComparison(Left.Evaluate(), Right.Evaluate(), Type);

                
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

        private object HandleComparison(object a, object b, TokenType type)
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
                    else if(type == TokenType.AND)
                    {
                        return (bool)a && (bool)b;
                    }
                    else if (type == TokenType.OR)
                    {
                        return (bool)a || (bool)b;
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
