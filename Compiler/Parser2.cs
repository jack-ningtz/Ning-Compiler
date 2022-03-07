using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler
{
    class Parser2
    {
        public Token token { get; set; }
        public Lexer lexer { get; set; }
        //Token的运算符优先级
        private readonly int[] OpPrec = {
            0,  // T_EOF
            10, // T_PLUS
            10, // T_MINUS
            20, // T_STAR
            20, // T_SLASH
            30, 30, //T_EQ, T_NE
            40, 40, 40, 40, // T_LT, T_GT, T_LE, T_GE
            0   // T_INTLIT
        };
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
                case Enum.T_IDENT:
                    int id = new SymbolTables().FindGlob(Lexer.TEXT);
                    if (id == -1)
                        Error.Fatals($"Unknow variable ",Lexer.TEXT,Lexer.line);
                    AST n = ast.MkAstLeaf(ASTEnum.A_IDENT, id);
                    token = lexer.Scan();

                    return n;
                default:
                    Error.Fatal("Syntax error", Lexer.line);
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
                case Enum.T_STAR:
                    return ASTEnum.A_MULTIPLY;
                case Enum.T_SLASH:
                    return ASTEnum.A_DIVIDE;
                case Enum.T_EQ:
                    return ASTEnum.A_EQ;
                case Enum.T_NE:
                    return ASTEnum.A_NE;
                case Enum.T_LT:
                    return ASTEnum.A_LT;
                case Enum.T_GT:
                    return ASTEnum.A_GT;
                case Enum.T_LE:
                    return ASTEnum.A_LE;
                case Enum.T_GE:
                    return ASTEnum.A_GE;
                default:
                    Error.Fatald("Syntax error , token", tokentype, Lexer.line);
                    return ASTEnum.A_ERROR;
            }
        }
        //检查运算符，返回优先级
        public int Op_Precedence(Enum tokentype)
        {
            int prec = OpPrec[(int)tokentype];
            if (prec == 0)
            {
                Error.Fatald("Syntax error , token", tokentype, Lexer.line);
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
            if (tokentype == Enum.T_SEMI)
            { 
                return left;
            }
            while (Op_Precedence(tokentype) > ptp)
            {
                token = lexer.Scan();
                right = Binexpr(OpPrec[(int)tokentype]);
                left = ast.MkAst(ArithOp(tokentype), left, right, 0);
                tokentype = token.token;
                if (tokentype == Enum.T_SEMI)
                {
                    return left;
                }
            }
            return left;
        }
    }
}