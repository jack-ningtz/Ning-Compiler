using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler
{
    class Miscellaneous
    {
        public Parser2 parser { get; set; }
        public Miscellaneous(Parser2 parser)
        {
            this.parser = parser;
        }
        
        public void Match(Enum t, string what)
        {
            
            if (parser.token.token == t)
            {
                parser.token = parser.lexer.Scan() ;
            }
            else
            {
                Console.WriteLine(parser.token.token + " " + t);
                Error.Fatals("Expecte", what, Lexer.line);
            }
        }
        public void Semi()
        {
            Match(Enum.T_SEMI, ";");
        }
        public void Lbrace()
        {
            Match(Enum.T_LBRACE, "{");
        }
        public void Rbrace()
        {
            Match(Enum.T_RBRACE, "}");
        }
        public void LParen()
        {
            Match(Enum.T_LPAREN, "(");
        }
        public void RParen()
        {
            Match(Enum.T_RPAREN, ")");
        }
        public void Ident()
        {
            Match(Enum.T_IDENT, "identifier");
        }
    }
}
