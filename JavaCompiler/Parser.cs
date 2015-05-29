using System;
using System.Collections.Generic;
using System.IO;

namespace JavaCompiler
{
    using Grammar = Dictionary<object, List<string>>;
    using TokenStream = List<Token>;

    class Parser
    {
        public class Node
        {
            public object Value { get; set; }
            public Node Left { get; set; }
            public Node Right { get; set; }

            public Node(object val, Node l = null, Node r = null)
            {
                Value = val;
                Left = l;
                Right = r;
            }

            public String Traverse(int depth = 0)
            {
                string res = "";
                for (int i = 0; i < depth; ++i)
                    res += " ";
                res += Value.ToString();
                if (Left != null) res += "\n" + Left.Traverse(depth + 1);
                if (Right != null) res += "\n" + Right.Traverse(depth + 1);
                return res;
            }

            public void Traverse(Action<Node> func)
            {
                func(this);
                if (Left != null) Left.Traverse(func);
                if (Right != null) Right.Traverse(func);
            }

            public override string ToString()
            {
                return Traverse();
            }
        }

        private Grammar grammar;

        public Parser(Grammar gr)
        {
            grammar = gr;
        }

        public Node Analize(TokenStream str)
        {
            var n = str.Count + 1;
            var table = new List<Node>[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    table[i, j] = new List<Node>(3);

            for (int i = 1; i < n; i++)
            {
                if (grammar.ContainsKey(str[i - 1].Name))
                    foreach (var item in grammar[str[i - 1].Name])
                        table[i - 1, i].Add(new Node(item, new Node(str[i-1].Value)));
                for (int j = i - 2; j >= 0; j--)
                    for (int k = j + 1; k <= i - 1; k++)
                    {
                        if (table[j, k] == null || table[k, i] == null || table[j, k].Count == 0 || table[k, i].Count == 0) continue;
                        foreach (var val in table[j, k])
                        {
                            foreach (var val2 in table[k, i])
                            {
                                var production = Tuple.Create<string, string>(val.Value.ToString(), val2.Value.ToString());
                                if (grammar.ContainsKey(production))
                                    foreach (var item in grammar[production])
                                        table[j, i].Add(new Node(item, val, val2));
                            }
                        }
                    }
            }
            return table[0, n - 1].Count > 0 ? table[0, n-1][0] : null;
        }

        public static Grammar GetGrammar(StreamReader stream)
        {
            var grammar = new Grammar();
            while (stream.Peek() != -1)
            {
                var line = stream.ReadLine().Trim();
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//")) continue;
                if (line.StartsWith("/*")) 
                    while (!line.EndsWith("*/")) 
                    {
                        if (stream.Peek() == -1)
                            throw new IOException("Unclosed multiline comment in grammar file.");
                        line = stream.ReadLine().Trim();
                    }

                var str = line.Split(new string[] { " ", "::=", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                if (str.Length == 2)
                    if (grammar.ContainsKey(str[1])) grammar[str[1]].Add(str[0]);
                    else grammar[str[1]] = new List<string> { str[0] };
                else if (str.Length == 3)
                    if (grammar.ContainsKey(Tuple.Create<string, string>(str[1], str[2]))) grammar[Tuple.Create<string, string>(str[1], str[2])].Add(str[0]);
                    else grammar[Tuple.Create<string, string>(str[1], str[2])] = new List<string> { str[0] };
                else throw new IOException("Cannot parse grammar file.");
            }
            return grammar;
        }
    }
}
