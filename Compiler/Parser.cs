using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler
{
    class Parser
    {
        private Token token;
        private Lexer lexer;
        public Parser(Token token,Lexer lexer)
        {
            this.token = token;
            this.lexer = lexer;
        }
        public AST Primary()
        {
            AST ast = new AST();
            // return int
            switch (token.token)
            {
                case Enum.T_INTLIT:
                    AST a = ast.MkAstLeaf(ASTEnum.A_INTLIT, token.value);
                    token = lexer.Scan();
                    return a;
                default:
                    Console.WriteLine($" syntax error on line {lexer.line}");
                    return null;
            }
        }
        public ASTEnum ArithOp(Enum tok)
        {
            switch (tok)
            {
                case Enum.T_PLUS:
                    return ASTEnum.A_ADD;
                case Enum.T_MINUS:
                    return ASTEnum.A_SUBTRACT;
                case Enum.T_START:
                    return ASTEnum.A_MULTIPLY;
                case Enum.T_SLASH:
                    return ASTEnum.A_DIVIDE;
                default:
                    Console.WriteLine($"unknown token in arithop() on line {lexer.line}");
                    return ASTEnum.A_ERROR;
            }
        }
        public AST Binexpr()
        {
            AST ast = new AST();
            AST n, left, right;
            ASTEnum nodetype;
            left = Primary();
            if (token.token == Enum.T_EOF)
            {
                return left;
            }
            nodetype = ArithOp(token.token);
            token = lexer.Scan();
            right = Binexpr();
   
            n = ast.MkAst(nodetype, left, right,0);
            return n;
        }
    }
}