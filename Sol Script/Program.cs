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

            Stack<Token> expression = parser.ConvertToPrefix(tokens);

            
            Node AST_Root = new Node(expression);

            Interpreter interpreter = new Interpreter();

            interpreter.InterpretExpression(AST_Root);
            

            return 0;
        }
    }
}
