using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler
{
    class Statement
    {
        public int NOREG = -1; //当AST生成函数没有寄存器可返回时，使用NOREG  
        public AST ast { get; set; }
        private int reg;
        public Parser2 parser { get; set; }
        Miscellaneous miscellaneous;
        public Statement(Parser2 parser)
        {
            this.parser = parser;
            this.miscellaneous = new Miscellaneous(this.parser);
        }
        // compound_statement:          // empty, i.e. no statement
        //      |      statement
        //      |      statement statements
        //      ;
        //
        // statement: print_statement
        //      |     declaration
        //      |     assignment_statement
        //      |     if_statement
        //      ;
        //
        // print_statement: 'print' expression ';'  ;
        //
        // declaration: 'int' identifier ';'  ;
        //
        // assignment_statement: identifier '=' expression ';'   ;
        //
        // if_statement: if_head
        //      |        if_head 'else' compound_statement
        //      ;
        //
        // if_head: 'if' '(' true_false_expression ')' compound_statement  ;
        //
        // identifier: T_IDENT ;   ;
        public AST Print_Statement()
        {
            AST n = new AST();
            AST tree;
            miscellaneous.Match(Enum.T_PRINT,"print");
            tree = parser.Binexpr(0);
            tree = n.MkAstUnary(ASTEnum.A_PRINT,tree,0);
            //Gen.GenPrintInt(reg);
            //Gen.GenFreeregs();
           // miscellaneous.Semi();
            return tree;
        }
        public void Var_Declaration()
        {
            miscellaneous.Match(Enum.T_INT, "int");
            miscellaneous.Ident();
            new SymbolTables().AddGlob(Lexer.TEXT);
            Gen.GenGlobSym(Lexer.TEXT);
            miscellaneous.Semi();
        }
        public AST Assignment_Statement()
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
            miscellaneous.Match(Enum.T_ASSIGN, "=");
            left = parser.Binexpr(0);
            // tree = n.MkAst(ASTEnum.A_ASSIGN, left, right, 0);
            tree = n.MkAst(ASTEnum.A_ASSIGN, left, null, right,0);
            //Gen.GenAST(tree, -1);
            //Gen.GenFreeregs();
            // miscellaneous.Semi();
            return tree;
        }
        // if_statement: if_head
        //      |        if_head 'else' compound_statement
        //      ;
        //
        // if_head: 'if' '(' true_false_expression ')' compound_statement  ;
        //
        // Parse an IF statement including
        // any optional ELSE clause
        // and return its AST
        public AST IF_Statement()
        {
            AST n = new AST();
            AST condast, trueast, falseast = null;
            miscellaneous.Match(Enum.T_IF, "if");
            miscellaneous.LParen();
            condast = parser.Binexpr(0);
            if ((condast.op < ASTEnum.A_EQ )|| (condast.op > ASTEnum.A_GE))
                Error.Fatal_General("Bad comparison operator");
            miscellaneous.RParen();
            trueast = Compound_Statement();
            if (parser.token.token == Enum.T_ELSE)
            {
                parser.token = parser.lexer.Scan();
                falseast = Compound_Statement();
            }
            return n.MkAst(ASTEnum.A_IF, condast, trueast, falseast, 0);
        }
        // while_statement: 'while' '(' true_false_expression ')' compound_statement  ;
        //
        // Parse a WHILE statement
        // and return its AST
        public AST While_Statement()
        {
            AST n = new AST();
            AST condAST, bodyAST;
            miscellaneous.Match(Enum.T_WHILE, "while");
            miscellaneous.LParen();
            condAST = parser.Binexpr(0);
            if ((condAST.op < ASTEnum.A_EQ) || (condAST.op > ASTEnum.A_GE))
                Error.Fatal_General("Bad comparison operator");
            miscellaneous.RParen();
            bodyAST = this.Compound_Statement();
            return n.MkAst(ASTEnum.A_WHILE, condAST, null, bodyAST, 0);

        }
        // for_statement: 'for' '(' preop_statement ';'
        //                          true_false_expression ';'
        //                          postop_statement ')' compound_statement  ;
        //
        // preop_statement:  statement          (for now)
        // postop_statement: statement          (for now)
        //
        // Parse a FOR statement
        // and return its AST
        public AST For_Statement()
        {
            AST n = new AST();
            AST condAST, bodyAST;
            AST preopAST, postopAST;
            AST tree;
            miscellaneous.Match(Enum.T_FOR, "for");
            miscellaneous.LParen();

            preopAST = Single_Statement();
            miscellaneous.Semi();

            condAST = parser.Binexpr(0);
            if ((condAST.op < ASTEnum.A_EQ) || (condAST.op > ASTEnum.A_GE))
                Error.Fatal_General("Bad comparison operator");
            miscellaneous.Semi();
            postopAST = Single_Statement();
            miscellaneous.RParen();
            bodyAST = Compound_Statement();

            tree = n.MkAst(ASTEnum.A_GLUE, bodyAST, null, postopAST, 0);
            tree = n.MkAst(ASTEnum.A_WHILE, condAST, null, tree, 0);
            tree = n.MkAst(ASTEnum.A_GLUE, preopAST, null, tree, 0);
            return tree;
        }
        public AST Single_Statement()
        {
            switch (parser.token.token)
            {
                case Enum.T_PRINT:
                    return this.Print_Statement();
                case Enum.T_INT:
                    this.Var_Declaration();
                    return null;
                case Enum.T_IDENT:
                    return this.Assignment_Statement();
                case Enum.T_IF:
                    return IF_Statement();
                case Enum.T_WHILE:
                    return this.While_Statement();
                case Enum.T_FOR:
                    return this.For_Statement();
                default:
                    Error.Fatald("Syntax error, token", parser.token.token, Lexer.line);
                    return null;
            }
        }
        public AST Compound_Statement()
        {
            AST n = new AST();

            AST left = null, tree;
            miscellaneous.Lbrace();
            while (true)
            {
                tree = Single_Statement();
                if ((tree != null) && ((tree.op == ASTEnum.A_PRINT) || (tree.op == ASTEnum.A_ASSIGN)))
                {
                    miscellaneous.Semi();
                }
                if (tree != null)
                {
                    if (left == null)
                        left = tree;
                    else
                        left = n.MkAst(ASTEnum.A_GLUE, left, null, tree, 0);
                }
                if (parser.token.token == Enum.T_RBRACE)
                {
                    miscellaneous.Rbrace();
                    return (left);
                }
            }
            //            while (true)
            //            {
            //                switch (parser.token.token)
            //                {
            //                    case Enum.T_PRINT:
            //                        tree = this.Print_Statement();
            //                        break;
            //                    case Enum.T_INT:
            //                        this.Var_Declaration();
            //                        tree = null;
            //                        break;
            //                    case Enum.T_IDENT:
            //                        tree = this.Assignment_Statement();
            //                        break;
            //                    case Enum.T_IF:
            //                        tree = IF_Statement();
            //                        break;
            //                    case Enum.T_WHILE:
            //                        tree = this.While_Statement();
            //                        break;
            //                    case Enum.T_RBRACE:
            //                        miscellaneous.Rbrace();
            //                        return left;
            //;                
            //                    default:
            //                        Error.Fatald("Syntax error, token", parser.token.token, Lexer.line);
            //                        return null;

            //                }
            //                if (tree != null)
            //                {
            //                    if (left == null)
            //                        left = tree;
            //                    else
            //                        left = n.MkAst(ASTEnum.A_GLUE, left, null, tree, 0);
            //                }
            //            }
        }
        //public void Statements()
        //{
            
        //    while (true)
        //    {
        //        switch (parser.token.token)
        //        {
        //            case Enum.T_PRINT:
        //                this.Print_Statement();
        //                break;
        //            case Enum.T_INT:
        //                this.Var_Declaration();
        //                break;
        //            case Enum.T_IDENT:
        //                this.Assignment_Statement();
        //                break;
        //            case Enum.T_EOF:
        //                return;
        //            default:
        //                Error.Fatald("Syntax error, token", parser.token.token, Lexer.line);
        //                return;

        //        }
        //    }
        //}


    }
}
