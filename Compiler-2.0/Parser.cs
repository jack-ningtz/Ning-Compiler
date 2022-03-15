//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Compiler
//{
//    class Parser
//    {
//        private Token token;
//        private Lexer lexer;

//        public Parser(Token token, Lexer lexer)
//        {
//            this.token = token;
//            this.lexer = lexer;
//        }
//        public AST Primary()
//        {
//            AST ast = new AST();
//            // return int
//            switch (token.token)
//            {
//                case Enum.T_INTLIT:
//                    AST a = ast.MkAstLeaf(ASTEnum.A_INTLIT, token.value);
//                    token = lexer.Scan();
//                    return a;
//                default:
//                    Console.WriteLine($" syntax error on line {Lexer.line}");
//                    Environment.Exit(0);
//                    return null;
//            }
//        }
//        public ASTEnum ArithOp(Enum tok)
//        {
//            switch (tok)
//            {
//                case Enum.T_PLUS:
//                    return ASTEnum.A_ADD;
//                case Enum.T_MINUS:
//                    return ASTEnum.A_SUBTRACT;
//                case Enum.T_STAR:
//                    return ASTEnum.A_MULTIPLY;
//                case Enum.T_SLASH:
//                    return ASTEnum.A_DIVIDE;
//                default:
//                    Console.WriteLine($"syntax error on line {Lexer.line}, token {tok}");
//                    Environment.Exit(0);
//                    return ASTEnum.A_ERROR;
//            }
//        }
//        // 返回根为 "*" 或 "/"操作符的AST树
//        public AST Multiplicative_Expr()
//        {
//            AST ast = new AST();
//            AST left, right;
//            Enum tokentype;
//            left = Primary();
//            tokentype = token.token;
//            if (tokentype == Enum.T_EOF)
//                return left;
//            while ((tokentype == Enum.T_STAR) || (tokentype == Enum.T_SLASH))
//            {
//                token = lexer.Scan();
//                right = Primary();
//                left = ast.MkAst(ArithOp(tokentype), left, right, 0);
//                tokentype = token.token;
//                if (tokentype == Enum.T_EOF)
//                    break;
//            }
//            return left;

//        }
//        //返回父节点为 "+" 或 "-"的AST树
//        public AST Additive_Expr()
//        {
//            AST ast = new AST();
//            AST left, right;
//            Enum tokentype;
//            left = Multiplicative_Expr();
//            tokentype = token.token;
//            if (tokentype == Enum.T_EOF)
//            {
//                return left;
//            }
//            while (true)
//            {
//                token = lexer.Scan();
//                right = Multiplicative_Expr();
//                left = ast.MkAst(ArithOp(tokentype), left, right, 0);
//                tokentype = token.token;
//                if (tokentype == Enum.T_EOF)
//                {
//                    break;
//                }
//            }
//            return left;
//        }
//        public AST Binexpr(int ptp)
//        {
//            return Additive_Expr();
//        }
//    }
//}