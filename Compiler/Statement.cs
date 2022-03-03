using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler
{
    class Statement
    {
        public AST ast { get; set; }
        private int reg;
        public Parser2 parser { get; set; }
        
        public Statement(Parser2 parser)
        {
            this.parser = parser;
        }
        // statements: statement
        //      | statement statements
        //      ;
        //
        // statement: 'print' expression ';'
        //      ;


        // Parse one or more statements
        public void Statements()
        {
            Miscellaneous miscellaneous = new Miscellaneous(parser);
            while (true)
            {
                miscellaneous.Match(Enum.T_PRINT, "print");
                ast = parser.Binexpr(0);
                reg = Gen.GenAST(ast);
                Gen.GenPrintInt(reg);
                Gen.GenFreeregs();
                miscellaneous.Semi();
                if (parser.token.token == Enum.T_EOF)
                    return;
            }
        }
    }
}
