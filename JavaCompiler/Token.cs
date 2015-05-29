using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaCompiler
{
    class Token
    {
        public enum TokenType
        {
            Error,
            Lambda,
            Keyword,
            Operator,
            Separator,
            Identifier,
            NullLiteral,
            CharLiteral,
            FloatLiteral,
            StringLiteral,
            IntegerLiteral,
            BooleanLiteral
        };

        public TokenType Type { get; private set; }
        public object Value { get; private set; }
        public string Name
        {
            get
            {
                if (Type == TokenType.Identifier || Type == TokenType.IntegerLiteral ||
                    Type == TokenType.FloatLiteral || Type == TokenType.BooleanLiteral ||
                    Type == TokenType.CharLiteral || Type == TokenType.StringLiteral)
                    return Type.ToString();
                return Value.ToString();
            }
        }

        public Token(TokenType type, object value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return Type.ToString();
        }

        #region Predefined Symbols

        public static Token BOOL_EQ = new Token(TokenType.Operator, "==");
        public static Token EQ = new Token(TokenType.Operator, "=");
        public static Token BOOL_NEQ = new Token(TokenType.Operator, "!=");
        public static Token BOOL_NOT = new Token(TokenType.Operator, "!");
        public static Token BOOL_GRT = new Token(TokenType.Operator, ">");
        public static Token BOOL_GEQ = new Token(TokenType.Operator, ">=");
        public static Token R_SHIFT_SGN = new Token(TokenType.Operator, ">>");
        public static Token R_SHIFT_SGN_EQ = new Token(TokenType.Operator, ">>=");
        public static Token R_SHIFT_USGN = new Token(TokenType.Operator, ">>>");
        public static Token R_SHIFT_USGN_EQ = new Token(TokenType.Operator, ">>>=");
        public static Token BOOL_LST = new Token(TokenType.Operator, "<");
        public static Token BOOL_LEQ = new Token(TokenType.Operator, "<=");
        public static Token L_SHIFT_SGN = new Token(TokenType.Operator, "<<");
        public static Token L_SHIFT_SGN_EQ = new Token(TokenType.Operator, "<<=");
        public static Token L_SHIFT_USGN = new Token(TokenType.Operator, "<<<");
        public static Token L_SHIFT_USGN_EQ = new Token(TokenType.Operator, "<<<=");
        public static Token MINUS = new Token(TokenType.Operator, "-");
        public static Token DECREMENT = new Token(TokenType.Operator, "--");
        public static Token MINUS_EQ = new Token(TokenType.Operator, "-=");
        public static Token ARROW = new Token(TokenType.Operator, "->");
        public static Token PLUS = new Token(TokenType.Operator, "+");
        public static Token INCREMENT = new Token(TokenType.Operator, "++");
        public static Token PLUS_EQ = new Token(TokenType.Operator, "+=");
        public static Token BIT_AND = new Token(TokenType.Operator, "&");
        public static Token BOOL_AND = new Token(TokenType.Operator, "&&");
        public static Token BIT_AND_EQ = new Token(TokenType.Operator, "&=");
        public static Token BIT_OR = new Token(TokenType.Operator, "|");
        public static Token BOOL_OR = new Token(TokenType.Operator, "||");
        public static Token BIT_OR_EQ = new Token(TokenType.Operator, "|=");
        public static Token MULT = new Token(TokenType.Operator, "*");
        public static Token MULT_EQ = new Token(TokenType.Operator, "*=");
        public static Token DIV = new Token(TokenType.Operator, "/");
        public static Token DIV_EQ = new Token(TokenType.Operator, "/=");
        public static Token MOD = new Token(TokenType.Operator, "%");
        public static Token MOD_EQ = new Token(TokenType.Operator, "%=");
        public static Token XOR = new Token(TokenType.Operator, "^");
        public static Token XOR_EQ = new Token(TokenType.Operator, "^=");
        public static Token NOT = new Token(TokenType.Operator, "~");
        public static Token TERNARY_THEN = new Token(TokenType.Operator, "?");
        public static Token TERNARY_ELSE = new Token(TokenType.Operator, ":");

        public static Token L_PAR = new Token(TokenType.Separator, "(");
        public static Token R_PAR = new Token(TokenType.Separator, ")");
        public static Token L_BRACE = new Token(TokenType.Separator, "{");
        public static Token R_BRACE = new Token(TokenType.Separator, "}");
        public static Token L_BRACKET = new Token(TokenType.Separator, "[");
        public static Token R_BRACKET = new Token(TokenType.Separator, "]");
        public static Token SEMICOLON = new Token(TokenType.Separator, ";");
        public static Token COMMA = new Token(TokenType.Separator, ",");
        public static Token DOT = new Token(TokenType.Separator, ".");
        public static Token ELIPSIS = new Token(TokenType.Separator, "...");
        public static Token AT = new Token(TokenType.Separator, "@");
        public static Token DOUBLE_COLON = new Token(TokenType.Separator, "::");
        
        public static Token LAMBDA = new Token(TokenType.Lambda, "\u03bb");

        public static Dictionary<string, Token> Keywords = new Dictionary<string, Token>(53)
        {
            { "abstract", new Token(TokenType.Keyword, "abstract") },
            { "assert", new Token(TokenType.Keyword, "assert") },
            { "boolean", new Token(TokenType.Keyword, "boolean") },
            { "break", new Token(TokenType.Keyword, "break") },
            { "byte", new Token(TokenType.Keyword, "byte") },  
            { "case", new Token(TokenType.Keyword, "case") },    
            { "catch", new Token(TokenType.Keyword, "catch") },   
            { "char", new Token(TokenType.Keyword, "char") },    
            { "class", new Token(TokenType.Keyword, "class") },   
            { "const", new Token(TokenType.Keyword, "const") },   
            { "continue", new Token(TokenType.Keyword, "continue") },
            { "default", new Token(TokenType.Keyword, "default") }, 
            { "do", new Token(TokenType.Keyword, "do") },      
            { "double", new Token(TokenType.Keyword, "double") },  
            { "else", new Token(TokenType.Keyword, "else") },    
            { "enum", new Token(TokenType.Keyword, "enum") },    
            { "extends", new Token(TokenType.Keyword, "extends") }, 
            { "final", new Token(TokenType.Keyword, "final") },   
            { "finally", new Token(TokenType.Keyword, "finally") }, 
            { "float", new Token(TokenType.Keyword, "float") },   
            { "for", new Token(TokenType.Keyword, "for") },       
            { "goto", new Token(TokenType.Keyword, "goto") },      
            { "if", new Token(TokenType.Keyword, "if") },        
            { "implements", new Token(TokenType.Keyword, "implements") },
            { "import", new Token(TokenType.Keyword, "import") },    
            { "instanceof", new Token(TokenType.Keyword, "instanceof") },
            { "int", new Token(TokenType.Keyword, "int") },       
            { "interface", new Token(TokenType.Keyword, "interface") }, 
            { "long", new Token(TokenType.Keyword, "long") },      
            { "native", new Token(TokenType.Keyword, "native") },    
            { "new", new Token(TokenType.Keyword, "new") },      
            { "package", new Token(TokenType.Keyword, "package") },  
            { "private", new Token(TokenType.Keyword, "private") },  
            { "protected", new Token(TokenType.Keyword, "protected") },
            { "public", new Token(TokenType.Keyword, "public") },   
            { "return", new Token(TokenType.Keyword, "return") },   
            { "short", new Token(TokenType.Keyword, "short") },    
            { "static", new Token(TokenType.Keyword, "static") },   
            { "strictfp", new Token(TokenType.Keyword, "strictfp") }, 
            { "super", new Token(TokenType.Keyword, "super") },    
            { "switch", new Token(TokenType.Keyword, "switch") },  
            { "synchronized", new Token(TokenType.Keyword, "synchronized") },
            { "this", new Token(TokenType.Keyword, "this") },
            { "throw", new Token(TokenType.Keyword, "throw") }, 
            { "throws", new Token(TokenType.Keyword, "throws") },
            { "transient", new Token(TokenType.Keyword, "transient") },
            { "try", new Token(TokenType.Keyword, "try") },
            { "void", new Token(TokenType.Keyword, "void") },
            { "volatile", new Token(TokenType.Keyword, "volatile") },
            { "while", new Token(TokenType.Keyword, "while") },
            { "true", new Token(TokenType.BooleanLiteral, "true") },
            { "false", new Token(TokenType.BooleanLiteral, "false") },
            { "null", new Token(TokenType.NullLiteral, "null") }
        };

        #endregion
    }
}
