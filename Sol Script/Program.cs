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

            Parser parser = new Parser();

            
            try
            {
                Stack<Token> expression = parser.Parse(tokens);
                Node AST_Root = Node.Build(expression);
                AST_Root.Evaluate();
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }

            return 0;
        }
    }
}
