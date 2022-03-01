using System;
using System.Collections.Generic;
using System.Text;

namespace Sol_Script
{
    class Interpreter
    {
        public float InterpretExpression(Node expressionRoot)
        {
            if (expressionRoot is NumberNode)
            {
                return (expressionRoot as NumberNode).Value;
            }
            else if (expressionRoot is Node)
            {
                float a = InterpretExpression(expressionRoot.Left);
                float b = InterpretExpression(expressionRoot.Right);

                switch(expressionRoot.Type)
                {
                    case TokenType.PLUS:
                        return a + b;

                    case TokenType.MINUS:
                        return a - b;

                    case TokenType.STAR:
                        return a * b;

                    case TokenType.SLASH:
                        return a / b;
                }
            }

            throw new Exception("You should not be here!");
        }
    }
}
