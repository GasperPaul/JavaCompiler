using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JavaCompiler
{
    using TokenStream = List<Token>;

    class Lexer
    {
        private int lineNum = 1;
        private char next;
        private TokenStream tokens = new TokenStream();
        private StreamReader programText;

        public Lexer(StreamReader _sr)
        {
            programText = _sr;
            readNext();
        }

        private char readNext()
        {
            return next = (char)programText.Read();
        }

        private bool readNext(char c)
        {
            if (readNext() == c)
            {
                readNext();
                return true;
            }
            return false;
        }

        private bool readNext(string str)
        {
            if (str.Contains(readNext()))
            {
                readNext();
                return true;
            }
            return false;
        }

        private bool EOF()
        {
            return programText.Peek() == -1;
        }

        public TokenStream Analize()
        {
            while (!EOF())
            {
                // skip all linebreaks and whitespace
                while (true)
                {
                    if (next == '\n')
                    {
                        readNext('\r');
                        lineNum++;
                    }
                    if (next == '\r') lineNum++;
                    if (char.IsWhiteSpace(next))
                        readNext();
                    else
                        break;
                }

                bool comment = false;
                var sb = new StringBuilder();
                var count = tokens.Count();

                // operators and separators
                switch (next)
                {
                    case '=':
                        if (readNext('=')) tokens.Add(Token.BOOL_EQ);
                        else tokens.Add(Token.EQ);
                        break;
                    case '!':
                        if (readNext('=')) tokens.Add(Token.BOOL_NEQ);
                        else tokens.Add(Token.BOOL_NOT);
                        break;
                    case '>':
                        if (readNext('=')) tokens.Add(Token.BOOL_GEQ);
                        else if (next == '>')
                        {
                            if (readNext('=')) tokens.Add(Token.R_SHIFT_SGN_EQ); 
                            else if (next == '>')
                            {
                                if (readNext('=')) tokens.Add(Token.R_SHIFT_USGN_EQ); 
                                else tokens.Add(Token.R_SHIFT_USGN);
                            }
                            else tokens.Add(Token.R_SHIFT_SGN);
                        }
                        else tokens.Add(Token.BOOL_GRT);
                        break;
                    case '<':
                        if (readNext('=')) tokens.Add(Token.BOOL_LEQ); 
                        else if (next == '<')
                        {
                            if (readNext('=')) tokens.Add(Token.L_SHIFT_SGN_EQ); 
                            else if (next == '<')
                            {
                                if (readNext('=')) tokens.Add(Token.L_SHIFT_USGN_EQ);
                                else tokens.Add(Token.L_SHIFT_USGN);
                            }
                            else tokens.Add(Token.L_SHIFT_SGN);
                        }
                        else tokens.Add(Token.BOOL_LST);
                        break;
                    case '-':
                        if (readNext('-')) tokens.Add(Token.DECREMENT);
                        else if (next == '=')
                        {
                            readNext();
                            tokens.Add(Token.MINUS_EQ);
                        }
                        else if (next == '>')
                        {
                            readNext();
                            tokens.Add(Token.ARROW);
                        }
                        else tokens.Add(Token.MINUS);
                        break;
                    case '+':
                        if (readNext('+')) tokens.Add(Token.INCREMENT);
                        else if (next == '=')
                        {
                            readNext();
                            tokens.Add(Token.PLUS_EQ);
                        }
                        else tokens.Add(Token.PLUS);
                        break;
                    case '&':
                        if (readNext('&')) tokens.Add(Token.BOOL_AND);
                        else if (next == '=')
                        {
                            readNext();
                            tokens.Add(Token.BIT_AND_EQ);
                        }
                        else tokens.Add(Token.BIT_AND);
                        break;
                    case '|':
                        if (readNext('|')) tokens.Add(Token.BOOL_OR);
                        else if (next == '=')
                        {
                            readNext();
                            tokens.Add(Token.BIT_OR_EQ);
                        }
                        else tokens.Add(Token.BIT_OR);
                        break;
                    case '*':
                        if (readNext('=')) tokens.Add(Token.MULT_EQ);
                        else tokens.Add(Token.MULT);
                        break;
                    case '/':
                        if (readNext('/'))
                        {
                            comment = true;
                            while (!readNext('\n')) ;
                        }
                        else if (next == '*')
                        {
                            comment = true;
                            while (true)
                            {
                                while (!readNext('*')) ;
                                if (next == '/') { readNext(); break; }
                            }
                        }
                        else if (next == '=') tokens.Add(Token.DIV_EQ);
                        else tokens.Add(Token.DIV);
                        break;
                    case '%':
                        if (readNext('=')) tokens.Add(Token.MOD_EQ);
                        else tokens.Add(Token.MOD);
                        break;
                    case '^':
                        if (readNext('=')) tokens.Add(Token.XOR_EQ);
                        else tokens.Add(Token.XOR);
                        break;
                    case ':':
                        if (readNext(':')) tokens.Add(Token.DOUBLE_COLON);
                        else tokens.Add(Token.TERNARY_ELSE);
                        break;
                    case '.':
                        if (readNext('.'))
                        {
                            if (readNext('.')) tokens.Add(Token.ELIPSIS);
                            else
                            {
                                tokens.Add(Token.DOT);
                                tokens.Add(Token.DOT);
                            }
                        }
                        else tokens.Add(Token.DOT);
                        break;
                    case '~':
                        readNext();
                        tokens.Add(Token.NOT);
                        break;
                    case '?':
                        readNext();
                        tokens.Add(Token.TERNARY_THEN);
                        break;
                    case '(':
                        readNext();
                        tokens.Add(Token.L_PAR);
                        break;
                    case ')':
                        readNext();
                        tokens.Add(Token.R_PAR);
                        break;
                    case '{':
                        readNext();
                        tokens.Add(Token.L_BRACE);
                        break;
                    case '}':
                        readNext();
                        tokens.Add(Token.R_BRACE);
                        break;
                    case '[':
                        readNext();
                        tokens.Add(Token.L_BRACKET);
                        break;
                    case ']':
                        readNext();
                        tokens.Add(Token.R_BRACKET);
                        break;
                    case ';':
                        readNext();
                        tokens.Add(Token.SEMICOLON);
                        break;
                    case ',':
                        readNext();
                        tokens.Add(Token.COMMA);
                        break;
                    case '@':
                        readNext();
                        tokens.Add(Token.AT);
                        break;
                    case '\'':
                        sb.Clear();
                        while (!readNext('\''))
                        {
                            if (next == '\\')
                            {
                                if (readNext('\''))
                                    sb.Append("\'");
                                else
                                {
                                    sb.Append("\\");
                                    sb.Append(next);
                                }
                            }
                            else sb.Append(next);
                        }
                        tokens.Add(new Token(Token.TokenType.CharLiteral, sb.ToString()));
                        break;
                    case '"':
                        sb.Clear();
                        while (!readNext('"'))
                        {
                            if (next == '\\')
                            {
                                if (readNext('"'))
                                    sb.Append("\\\"");
                                else
                                {
                                    sb.Append('\\');
                                    sb.Append(next);
                                }
                            }
                            else sb.Append(next);
                        }
                        tokens.Add(new Token(Token.TokenType.StringLiteral, sb.ToString()));
                        break;
                }
                if (tokens.Count() != count || comment) continue;

                // integers and floats
                if (char.IsDigit(next))
                {
                    var hexDigts = "abcdefABCDEF";

                    int numBase = 10;
                    if (next == '0')
                    {
                        if (readNext("xX")) numBase = 16;
                        else if (next == 'b' || next == 'B')
                        {
                            readNext();
                            numBase = 2;
                        }
                        else
                        {
                            sb.Append(next);
                            numBase = 8;
                        }
                    }

                    sb.Clear();
                    sb.Append(next);
                    while (char.IsDigit(readNext()) || (numBase == 16 && hexDigts.Contains(next)) || next == '_')
                        if (next != '_')
                            sb.Append(next);
                    var numeral = sb.ToString();
                    if (next == 'l' || next == 'L')
                        tokens.Add(new Token(Token.TokenType.IntegerLiteral, Convert.ToInt64(numeral, numBase)));
                    else
                    {
                        string floatPart = "";
                        if ((numBase == 10 || numBase == 16) && next == '.')
                        {
                            sb.Clear();
                            sb.Append(next);
                            while (char.IsDigit(readNext()) || (numBase == 16 && hexDigts.Contains(next)) || next == '_')
                                if (next != '_')
                                    sb.Append(next);
                            floatPart = sb.ToString();
                        }

                        string exponent = "";
                        if ((numBase == 16 && "pP".Contains(next)) || (numBase == 10 && "eE".Contains(next)))
                        {
                            sb.Clear();
                            if (readNext("+-") || char.IsDigit(next))
                                sb.Append(next);
                            while (char.IsDigit(readNext()) || next == '_')
                                if (next != '_')
                                    sb.Append(next);
                            exponent = sb.ToString();
                        }

                        if (string.IsNullOrWhiteSpace(floatPart) && string.IsNullOrWhiteSpace(exponent) && !("fFdD".Contains(next)))
                            tokens.Add(new Token(Token.TokenType.IntegerLiteral, Convert.ToInt32(numeral, numBase)));
                        else
                        {
                            if (numBase == 10)
                            {
                                numeral += floatPart + exponent;
                                if (next == 'f' || next == 'F')
                                    tokens.Add(new Token(Token.TokenType.FloatLiteral, float.Parse(numeral, System.Globalization.CultureInfo.InvariantCulture)));
                                else
                                    tokens.Add(new Token(Token.TokenType.FloatLiteral, double.Parse(numeral, System.Globalization.CultureInfo.InvariantCulture)));
                            }
                            else
                            {
                                double num = int.Parse(numeral, System.Globalization.NumberStyles.HexNumber);
                                double b = 16;
                                foreach (var ch in floatPart)
                                    if (ch != '.')
                                    {
                                        num += int.Parse(ch.ToString(), System.Globalization.NumberStyles.HexNumber) / b;
                                        b *= 16;
                                    }
                                num *= Math.Pow(2, Convert.ToDouble(exponent));
                                if (next == 'f' || next == 'F')
                                    tokens.Add(new Token(Token.TokenType.FloatLiteral, Convert.ToSingle(num)));
                                else
                                    tokens.Add(new Token(Token.TokenType.FloatLiteral, num));
                            }
                        }
                    }
                    continue;
                }

                // identifiers and keywords
                if (char.IsLetter(next))
                {
                    sb.Clear();
                    sb.Append(next.ToString());
                    while (char.IsLetterOrDigit(readNext()) || next == '_' || next == '$')
                        sb.Append(next);
                    var identifier = sb.ToString();
                    if (Token.Keywords.ContainsKey(identifier))
                        tokens.Add(Token.Keywords[identifier]);
                    else
                        tokens.Add(new Token(Token.TokenType.Identifier, identifier));
                    continue;
                }

                tokens.Add(new Token(Token.TokenType.Error, "Error: unknown symbol at line " + lineNum.ToString()));
            }

            tokens.Add(Token.LAMBDA);
            return tokens;
        }
    }
}
