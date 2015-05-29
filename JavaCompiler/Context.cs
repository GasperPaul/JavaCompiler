using System.Collections.Generic;
using System.Text;

namespace JavaCompiler
{
    class Context
    {
        public class Variable
        {
            public string Name { get; private set; }
            public string Type { get; private set; }
            public string Block { get; private set; }

            public Variable(string name, string type, string block)
            {
                Name = name;
                Type = type;
                Block = block;
            }
        }

        private List<Variable> namesTable = new List<Variable>();
        public IReadOnlyList<Variable> NamesTable { get { return namesTable; } }

        private List<string> errors = new List<string>();
        public IReadOnlyList<string> Errors { get { return errors; } }
        public bool HasErrors { get { return errors.Count != 0; } }

        private Stack<int> blocks = new Stack<int>();
        private int currentBlock = 0;
        private int nextBlock = 1;

        private string className = string.Empty;
        private string funcName = string.Empty;
        private string type = string.Empty;

        public IReadOnlyList<Variable> Analyse(JavaCompiler.Parser.Node tree)
        {
            tree.Traverse(node =>
            {
                string block = className + (string.IsNullOrWhiteSpace(funcName) ? string.Empty : ("::" + funcName + "::_" + currentBlock));
                switch (node.Value.ToString())
                {
                    case "block":
                        if (string.IsNullOrWhiteSpace(funcName)) break;
                        else
                        {
                            blocks.Push(currentBlock);
                            currentBlock = nextBlock;
                            nextBlock++;
                        }
                        break;
                    case "r_brace":
                        if (blocks.Count > 0)
                            currentBlock = blocks.Pop();
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(funcName))
                                funcName = string.Empty;
                            else
                                className = string.Empty;
                            currentBlock = 0;
                            nextBlock = 1;
                        }
                        break;
                    case "field_declaration":
                        type = string.Empty;
                        if (node.Right.Value.ToString() == "field_declaration") break;
                        else
                            node.Traverse(y =>
                            {
                                switch (y.Value.ToString())
                                {
                                    case "declarator":
                                        type = y.Left.Value.ToString();
                                        break;
                                    case "var_name":
                                        string name = y.Left.Value.ToString();
                                        if (namesTable.Exists(x => x.Name == name && x.Block == block))
                                        {
                                            var errorLine = GetErrorLine(node);
                                            errors.Add(string.Format("Redeclaration of variable {0}:\n{1}", name, errorLine));
                                            return;
                                        }
                                        namesTable.Add(new Variable(type, name, block));
                                        break;
                                }
                            });
                        break;
                    case "var_declaration":
                        type = string.Empty;
                        node.Traverse(y =>
                        {
                            switch (y.Value.ToString())
                            {
                                case "declarator":
                                    type = y.Left.Value.ToString();
                                    break;
                                case "var_name":
                                    string name = y.Left.Value.ToString();
                                    if (namesTable.Exists(x => x.Name == name && x.Block == block))
                                    {
                                        var errorLine = GetErrorLine(node);
                                        errors.Add(string.Format("Redeclaration of variable {0}:\n{1}", name, errorLine));
                                        return;
                                    }
                                    namesTable.Add(new Variable(type, name, block));
                                    break;
                            }
                        });
                        break;
                    case "class_declarator":
                        className = node.Right.Left.Value.ToString();
                        namesTable.Add(new Variable(node.Left.Left.Value.ToString(), className, "namespace"));
                        break;
                    case "func_declarator":
                        StringBuilder funcSignature = new StringBuilder("function ");
                        node.Right.Traverse(x =>
                        {
                            switch (x.Value.ToString())
                            {
                                case "declarator":
                                    funcSignature.Append(x.Left.Value.ToString());
                                    if (!string.IsNullOrWhiteSpace(funcName)) funcSignature.Append(' ');
                                    break;
                                case "void_token":
                                    funcSignature.Append(x.Left.Value.ToString());
                                    break;
                                case "func_name":
                                    funcName = x.Left.Value.ToString();
                                    funcSignature.Append(' ').Append(funcName).Append(' ');
                                    break;
                            }
                        });
                        if (namesTable.Exists(x => x.Name == funcName && x.Block == block && x.Type == funcSignature.ToString()))
                        {
                            var errorLine = GetErrorLine(node);
                            errors.Add(string.Format("Redeclaration of function {0}:\n{1}", funcName, errorLine));
                            return;
                        }
                        namesTable.Add(new Variable(funcName, funcSignature.ToString(), block));
                        break;
                    case "expression":
                        /*if (node.Right == null)
                            if ((node.Left.Value is string) && !namesTable.Exists(x => x.Name == node.Left.Value && x.Block == block))
                                errors.Add(string.Format("Undeclared variable {0}.", node.Left.Value));*/
                        break;
                }
            });

            return namesTable;
        }

        private string GetErrorLine(JavaCompiler.Parser.Node node)
        {
            StringBuilder errorLine = new StringBuilder();
            node.Traverse(x =>
            {
                if (x.Left == null && x.Right == null) errorLine.Append(x.Value.ToString()).Append(' ');
            });
            return errorLine.ToString();
        }
    }
}