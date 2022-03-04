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
        Miscellaneous miscellaneous;
        public Statement(Parser2 parser)
        {
            this.parser = parser;
            this.miscellaneous = new Miscellaneous(this.parser);
        }
        // Parsing of statements
        // Copyright (c) 2019 Warren Toomey, GPL3

        // statements: statement
        //      |      statement statements
        //      ;
        //
        // statement: 'print' expression ';'
        //      |     'int'   identifier ';'
        //      |     identifier '=' expression ';'
        //      ;
        //
        // identifier: T_IDENT
        //      ;
        public void Print_Statement()
        {
            miscellaneous.Match(Enum.T_PRINT,"print");
            ast = parser.Binexpr(0);
            reg = Gen.GenAST(ast, -1);
            Gen.GenPrintInt(reg);
            Gen.GenFreeregs();
            miscellaneous.Semi();
        }
        public void Var_Declaration()
        {
            miscellaneous.Match(Enum.T_INT, "int");
            miscellaneous.Ident();
            new SymbolTables().AddGlob(Lexer.TEXT);
            Gen.GenGlobSym(Lexer.TEXT);
            miscellaneous.Semi();
        }
        public void Assignment_Statement()
        {
            AST n = new AST();
            AST tree, left, right;
            miscellaneous.Ident();
            int id = new SymbolTables().FindGlob(Lexer.TEXT);
            if (id == -1)
            {
                Error.Fatals($"Undeclared variable",Lexer.TEXT,Lexer.line);
            }
            right = n.MkAstLeaf(ASTEnum.A_LVIDENT, id);
            miscellaneous.Match(Enum.T_EQUALS, "=");
            left = parser.Binexpr(0);
            tree = n.MkAst(ASTEnum.A_ASSIGN, left, right, 0);
            Gen.GenAST(tree, -1);
            Gen.GenFreeregs();
            miscellaneous.Semi();

        }

        public void Statements()
        {
            
            while (true)
            {
                switch (parser.token.token)
                {
                    case Enum.T_PRINT:
                        this.Print_Statement();
                        break;
                    case Enum.T_INT:
                        this.Var_Declaration();
                        break;
                    case Enum.T_IDENT:
                        this.Assignment_Statement();
                        break;
                    case Enum.T_EOF:
                        return;
                    default:
                        Error.Fatald("Syntax error, token", parser.token.token, Lexer.line);
                        return;

                }
            }
        }


    }
}
