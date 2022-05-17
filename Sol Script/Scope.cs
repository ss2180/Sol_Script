using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sol_Script
{
    class Scope
    {
        public Dictionary<string, object> variables { get; set; }
        public List<List<Token>> statements { get; set; }

        private Parser parser = new Parser();

        public Scope(List<List<Token>> listOfStatements)
        {
            statements = listOfStatements;
            variables = new Dictionary<string, object>();
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
                            i = HandleIf(statements, i) - 1;
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
        }

        private int HandleIf(List<List<Token>> statements, int index)
        {
            Stack<Token> expression = parser.Parse(statements[index]);
            Node AST_Root = Node.Build(expression, this);
            bool ifResult = (bool)AST_Root.Evaluate();

            int ifStatementCounter = 1;

            List<List<Token>> newStatements = new List<List<Token>>();

            index++;
            while (index < statements.Count && ifStatementCounter != 0)
            {
                if(statements[index][0].Type == TokenType.RIGHT_CURLY_BRACE)
                {
                    ifStatementCounter--;
                }
                else if(statements[index][0].Type == TokenType.IF)
                {
                    ifStatementCounter++;
                }

                newStatements.Add(statements[index]);

                index++;
            }

            if(ifResult)
            {
                Scope ifScope = new Scope(newStatements);
                ifScope.Run();
                return index;
            }
            else
            {
                return index - 1;
            }
        }
    }
}
