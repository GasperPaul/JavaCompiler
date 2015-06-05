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

                        StringBuilder code = new StringBuilder();
                        int tmpNum = 0;
                        int labelNum = 0;

                        Func<string> NewLabel = () => "IL_" + labelNum++;
                        Func<string> NewTempVar = () => "_t" + tmpNum++;

                        Action<string> Emit = (x) => code.Append(x).Append('\n');
                        Func<JavaCompiler.Parser.Node, string> Generate = (x) => "";
                        Generate = (node) =>
                        {
                            switch (node.Value.ToString())
                            {
                                case "expression":
                                case "bool_expr":
                                    if (node.Right == null)
                                    {
                                        var tmpVar = NewTempVar();
                                        Emit(string.Format("{0} = {1}", tmpVar, node.Left.Value));
                                        return tmpVar;
                                    }
                                    else if (node.Right.Value.ToString() == "operator_expression" || node.Right.Value.ToString() == "r_bool_expr")
                                    {
                                        var tmpVar = NewTempVar();
                                        var tmpLeft = Generate(node.Left);
                                        var tmpRight = Generate(node.Right.Right);
                                        Emit(string.Format("{0} = {1} {2} {3}", tmpVar, tmpLeft, node.Right.Left.Left, tmpRight));
                                        return tmpVar;
                                    }
                                    else if (node.Left.Value.ToString() == "l_paren")
                                    {
                                        return Generate(node.Right.Left);
                                    }
                                    else if (node.Right.Value.ToString() == "r_unary_operator")
                                    {
                                        var tmpVar = NewTempVar();
                                        var tmpLeft = node.Right.Left.Value.ToString().IndexOfAny("+-".ToArray()) != -1 ? node.Left.Left.Value : Generate(node.Left);
                                        Emit(string.Format("{0} = {1} {2}", tmpVar, tmpLeft, node.Right.Left));
                                        return tmpVar;
                                    }
                                    else if (node.Left.Value.ToString() == "l_unary_operator" || node.Left.Value.ToString() == "unary_not")
                                    {
                                        var tmpVar = NewTempVar();
                                        var tmpRight = node.Left.Left.Value.ToString().IndexOfAny("+-".ToArray()) != -1 ? node.Right.Left.Value : Generate(node.Right);
                                        Emit(string.Format("{0} = {1} {2}", tmpVar, node.Left.Left, tmpRight));
                                    }
                                    else if (node.Left.Value.ToString() == "ternary_part")
                                    {
                                        var tmpVar = NewTempVar();
                                        var labelTrue = NewLabel();
                                        var labelFalse = NewLabel();
                                        var ternaryCondition = Generate(node.Left);
                                        var tmpTrue = Generate(node.Right.Left.Right);
                                        var tmpFalse = Generate(node.Right.Right.Right);
                                        Emit(string.Format("IFZ {0} GOTO {1}", ternaryCondition, labelFalse));
                                        Emit(string.Format("{0} = {1}", tmpVar, tmpTrue));
                                        Emit(string.Format("GOTO {0}", labelTrue));
                                        Emit(string.Format("{0}:", labelFalse));
                                        Emit(string.Format("{0} = {1}", tmpVar, tmpFalse));
                                        Emit(string.Format("{0}:", labelTrue));
                                        return tmpVar;
                                    }
                                    break;
                                case "var_definition":
                                    var tmpVarExpr = Generate(node.Right.Right);
                                    Emit(string.Format("{0} = {1}", node.Left.Left, tmpVarExpr));
                                    break;
                                case "var_assignment":
                                    var tmpVarAss = Generate(node.Right.Value.ToString() == "r_var_assignment" ? node.Right.Left.Right : node.Right.Right);
                                    if (node.Right.Value.ToString() == "r_var_assignment")
                                    {
                                        var op = node.Right.Left.Left.Left.Value.ToString();
                                        op = op.Remove(op.Count() - 1);
                                        Emit(string.Format("{0} = {1} {2} {3}", node.Left.Left, node.Left.Left, op, tmpVarAss));
                                    }
                                    else
                                        Emit(string.Format("{0} = {1}", node.Left.Left, tmpVarAss));
                                    return node.Left.Left.Value.ToString();
                                case "short_if_statement":
                                    var labelShortAfter = NewLabel();
                                    var exprShortIf = Generate(node.Left.Right.Right.Left);
                                    Emit(string.Format("IFZ {0} GOTO {1}", exprShortIf, labelShortAfter));
                                    Generate(node.Right);
                                    Emit(string.Format("{0}:", labelShortAfter));
                                    break;
                                case "long_if_statement":
                                    var labelElse = NewLabel();
                                    var labelAfter = NewLabel();
                                    var exprLongIf = Generate(node.Left.Left.Right.Right.Left);
                                    Emit(string.Format("IFZ {0} GOTO {1}", exprLongIf, labelElse));
                                    Generate(node.Left.Right);
                                    Emit(string.Format("GOTO {0}", labelAfter));
                                    Emit(string.Format("{0}:", labelElse));
                                    Generate(node.Right.Right);
                                    Emit(string.Format("{0}:", labelAfter));
                                    break;
                                case "while_statement":
                                    var labelBeforeWhile = NewLabel();
                                    var labelAfterWhile = NewLabel();
                                    Emit(string.Format("{0}:", labelBeforeWhile));
                                    var whileExpr = Generate(node.Left.Right.Right.Left);
                                    Emit(string.Format("IFZ {0} GOTO {1}", whileExpr, labelAfterWhile));
                                    Generate(node.Right);
                                    Emit(string.Format("GOTO {0}", labelBeforeWhile));
                                    Emit(string.Format("{0}:", labelAfterWhile));
                                    break;
                                default:
                                    if (node.Left != null) Generate(node.Left);
                                    if (node.Right != null) Generate(node.Right);
                                    break;
                            }
                            return null;
                        };

                        Generate(tree);
                        Console.WriteLine();
                        Console.WriteLine(code.ToString());
                    }
                }
                else
                    Console.WriteLine("Syntax error.");
                Console.ReadLine();
            }
        }
    }
}