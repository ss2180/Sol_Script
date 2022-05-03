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

        public Scope(List<List<Token>> listOfStatements)
        {
            statements = listOfStatements;
            variables = new Dictionary<string, object>();
        }

        public void Run()
        {
            Parser parser = new Parser();

            try
            {
                foreach (List<Token> statement in statements)
                {
                    Stack<Token> expression = parser.Parse(statement);
                    Node AST_Root = Node.Build(expression, this);
                    AST_Root.Evaluate();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }
    }
}
