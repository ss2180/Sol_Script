using System;
using System.Collections.Generic;
using System.IO;

namespace Sol_Script
{
    class Program
    {
        private static int Main()
        {
            string source = File.ReadAllText("../../../source.sol");

            Scanner scanner = new Scanner();

            try
            {
                scanner.ScanLine(source);
            }
            catch(IOException e)
            {
                Console.WriteLine(e);

                return -1;
            }

            List<Token> tokens = scanner.Tokens;

            List<List<Token>> listOfStatements = new List<List<Token>>();
            int startIndex = 0;
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].Type == TokenType.NEWLINE || tokens[i].Type == TokenType.EOF)
                {
                    int endIndex = i;
                    listOfStatements.Add(new List<Token>(tokens.GetRange(startIndex, endIndex - startIndex)));
                    startIndex = i + 1;
                }
            }

            Parser parser = new Parser();

            try
            {
                foreach (List<Token> statement in listOfStatements)
                {
                    Stack<Token> expression = parser.Parse(statement);
                    Node AST_Root = Node.Build(expression);
                    AST_Root.Evaluate();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }

            return 0;
        }
    }
}
