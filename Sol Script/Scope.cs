using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sol_Script
{
    class Scope
    {
        public List<Dictionary<string, object>> variables = new List<Dictionary<string, object>>();
        public List<List<Token>> statements { get; set; }

        private Parser parser = new Parser();

        private bool ifExecuted = false;

        public Scope(List<List<Token>> listOfStatements)
        {
            statements = listOfStatements;
            variables.Add(new Dictionary<string, object>());
        }

        public Scope(List<List<Token>> listOfStatements, List<Dictionary<string, object>> externalVariables)
        {
            statements = listOfStatements;
            variables = externalVariables;
            variables.Add(new Dictionary<string, object>());
        }

        public void Run()
        {
            try
            {
                for (int i = 0; i < statements.Count; i++)
                {
                    switch (statements[i][0].Type)
                    {
                        case TokenType.IF:
                            i = HandleIf(statements, i);
                            break;
                        case TokenType.ELIF:
                            i = HandleElif(statements, i);
                            break;
                        case TokenType.ELSE:
                            i = HandleElse(statements, i);
                            break;
                        case TokenType.WHILE:
                            i = HandleWhile(statements, i);
                            break;
                        default:
                            
                            if (statements[i][0].Type != TokenType.LEFT_CURLY_BRACE && statements[i][0].Type != TokenType.RIGHT_CURLY_BRACE)
                            {
                                Stack<Token> expression = parser.Parse(statements[i]);
                                Node AST_Root = Node.Build(expression, this);
                                AST_Root.Evaluate();
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }

            variables.RemoveAt(variables.Count - 1);
        }

        private int HandleIf(List<List<Token>> statements, int index)
        {
            Stack<Token> expression = parser.Parse(statements[index]);
            Node AST_Root = Node.Build(expression, this);
            bool ifResult = (bool)AST_Root.Evaluate();
            ifExecuted = false;

            int scopeCounter = 1;

            List<List<Token>> newStatements = new List<List<Token>>();

            index++;
            while (index < statements.Count && scopeCounter != 0)
            {
                if(statements[index][0].Type == TokenType.RIGHT_CURLY_BRACE)
                {
                    scopeCounter--;
                }
                else if(statements[index][0].Type == TokenType.IF || statements[index][0].Type == TokenType.ELIF || statements[index][0].Type == TokenType.ELSE || statements[index][0].Type == TokenType.WHILE)
                {
                    scopeCounter++;
                }

                newStatements.Add(statements[index]);

                index++;
            }

            if(ifResult)
            {
                Scope ifScope = new Scope(newStatements, variables);
                ifScope.Run();
                ifExecuted = true;
                return index - 1;
            }
            else
            {
                return index - 1;
            }
        }

        private int HandleElif(List<List<Token>> statements, int index)
        {
            Stack<Token> expression = parser.Parse(statements[index]);
            Node AST_Root = Node.Build(expression, this);
            bool ifResult = (bool)AST_Root.Evaluate();

            int scopeCounter = 1;

            List<List<Token>> newStatements = new List<List<Token>>();

            index++;
            while (index < statements.Count && scopeCounter != 0)
            {
                if (statements[index][0].Type == TokenType.RIGHT_CURLY_BRACE)
                {
                    scopeCounter--;
                }
                else if (statements[index][0].Type == TokenType.IF || statements[index][0].Type == TokenType.ELIF || statements[index][0].Type == TokenType.ELSE || statements[index][0].Type == TokenType.WHILE)
                {
                    scopeCounter++;
                }

                newStatements.Add(statements[index]);

                index++;
            }

            if (!ifExecuted)
            {
                if (ifResult)
                {
                    Scope elifScope = new Scope(newStatements, variables);
                    elifScope.Run();
                    ifExecuted = true;
                }
            }

            return index - 1;
            
        }

        private int HandleElse(List<List<Token>> statements, int index)
        {
            int scopeCounter = 1;

            List<List<Token>> newStatements = new List<List<Token>>();

            index++;
            while (index < statements.Count && scopeCounter != 0)
            {
                if (statements[index][0].Type == TokenType.RIGHT_CURLY_BRACE)
                {
                    scopeCounter--;
                }
                else if (statements[index][0].Type == TokenType.IF || statements[index][0].Type == TokenType.ELIF || statements[index][0].Type == TokenType.ELSE || statements[index][0].Type == TokenType.WHILE)
                {
                    scopeCounter++;
                }

                newStatements.Add(statements[index]);

                index++;
            }

            if (!ifExecuted)
            {
                Scope elseScope = new Scope(newStatements, variables);
                elseScope.Run();
                return index - 1;
            }
            else
            {
                return index - 1;
            }
        }

        private int HandleWhile(List<List<Token>> statements, int index)
        {
            Stack<Token> expression = parser.Parse(statements[index]);
            Node AST_Root = Node.Build(expression, this);
            bool whileResult = (bool)AST_Root.Evaluate();

            int scopeCounter = 1;

            List<List<Token>> newStatements = new List<List<Token>>();

            index++;
            while (index < statements.Count && scopeCounter != 0)
            {
                if (statements[index][0].Type == TokenType.RIGHT_CURLY_BRACE)
                {
                    scopeCounter--;
                }
                else if (statements[index][0].Type == TokenType.IF || statements[index][0].Type == TokenType.ELIF || statements[index][0].Type == TokenType.ELSE || statements[index][0].Type == TokenType.WHILE)
                {
                    scopeCounter++;
                }

                newStatements.Add(statements[index]);

                index++;
            }

            if(whileResult)
            {
                while (whileResult)
                {
                    Scope whileScope = new Scope(newStatements, variables);
                    whileScope.Run();

                    whileResult = (bool)AST_Root.Evaluate();
                }
            }
            else
            {
                return index - 1;
            }


            return index - 1;
        }
    }
}
