using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1) return;
            Parser parser;
            using (var reader = new StreamReader(args.Length < 2 ? "java.grammar" : args[1]))
            {
                var grammar = Parser.GetGrammar(reader);
                parser = new Parser(grammar);
            }

            using (var reader = new StreamReader(args[0]))
            {
                var lexer = new Lexer(reader);
                var tokensStream = lexer.Analize();
                var tree = parser.Analize(tokensStream);

                foreach (var token in tokensStream)
                    Console.WriteLine("{0,-20} : {1}", token.ToString(), token.Value.ToString());
                Console.ReadLine();
                if (tree != null)
                {
                    Console.WriteLine(tree);
                    Console.WriteLine();

                    var context = new Context();
                    var namesTable = context.Analyse(tree);

                    if (context.HasErrors)
                        foreach (var s in context.Errors)
                            Console.WriteLine(s);
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("{0,-20} : {1, -30} : {2}", "Name", "Type", "Block name");
                        Console.WriteLine("----------------------------------------------------------------");
                        foreach (var variable in namesTable)
                        {
                            Console.WriteLine("{0,-20} : {1, -30} : {2}", variable.Name, variable.Type, variable.Block);
                        }
                    }
                }
                else
                    Console.WriteLine("Syntax error.");
                Console.ReadLine();
            }
        }
    }
}