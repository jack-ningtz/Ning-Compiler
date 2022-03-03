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
                Console.WriteLine($"{what} expected on line {parser.lexer.line} \n");
                Environment.Exit(1);
            }
        }
        public void Semi()
        {
            Match(Enum.T_SEMI, ";");
        }
    }
}
