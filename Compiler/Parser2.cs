using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler
{
    class Parser2
    {
        private Token token;
        private Lexer lexer;
        //Token的运算符优先级
        private readonly int[] OpPrec = { 0, 10, 10, 20, 20, 0 };
        public Parser2(Token token, Lexer lexer)
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
                    Environment.Exit(0);
                    return null;
            }
        }
        public ASTEnum ArithOp(Enum tokentype)
        {
            switch (tokentype)
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
                    Console.WriteLine($"syntax error on line {lexer.line}, token {tokentype}");
                    Environment.Exit(0);
                    return ASTEnum.A_ERROR;
            }
        }
        //检查运算符，返回优先级
        public int Op_Precedence(Enum tokentype)
        {
            int prec = OpPrec[(int)tokentype];
            if (prec == 0)
            {
                Console.WriteLine($"syntax error on line {lexer.line}, token {tokentype}");
                Environment.Exit(0);
                return 0;
            }
            return prec;
        }
        //public AST Binexpr()
        //{
        //    AST ast = new AST();
        //    AST n, left, right;
        //    ASTEnum nodetype;
        //    left = Primary();
        //    if (token.token == Enum.T_EOF)
        //    {
        //        return left;
        //    }
        //    nodetype = ArithOp(token.token);
        //    token = lexer.Scan();
        //    right = Binexpr();

        //    n = ast.MkAst(nodetype, left, right,0);
        //    return n;
        //}
        public AST Binexpr(int ptp)
        {
            AST ast = new AST();
            AST left, right;
            Enum tokentype;
            left = Primary();
            tokentype = token.token;
            if (tokentype == Enum.T_EOF)
            {
                return left;
            }
            while (Op_Precedence(tokentype) > ptp)
            {
                token = lexer.Scan();
                right = Binexpr(OpPrec[(int)tokentype]);
                left = ast.MkAst(ArithOp(tokentype), left, right, 0);
                tokentype = token.token;
                if (tokentype == Enum.T_EOF)
                {
                    return left;
                }
            }
            return left;
        }
    }
}