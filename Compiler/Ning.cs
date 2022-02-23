using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Compiler
{
    class Ning
    {
        public static void NingCompileFile(string filename)
        {
            FileStream stream = Io.ReadFile(filename);
            NingCompile(stream);
            stream.Close();
        }

        private static void NingCompile(FileStream stream)
        {
            //string[] tokstr = { "+", "-", "*", "/", "intlit" };
            Lexer lexer = new Lexer(stream);
            Token token = lexer.Scan();
            Parser parser = new Parser(token,lexer);
            AST n = parser.Binexpr();
            Console.WriteLine($"{InterpretAST(n)}");
            //while (token.token != Enum.T_EOF)
            //{ 
            //    Console.Write($"Token : {tokstr[(int)token.token]}")  ;
            //    if (token.token == Enum.T_INTLIT)
            //        Console.Write($" ,value {token.value}");
            //    Console.WriteLine();
            //    token = lexer.Scan();
            //}
        }
        static string[] tokstr = { "+", "-", "*", "/" };
        private static int InterpretAST(AST ast)
        {
            int leftval = 0, rightval = 0;

            if (ast.left != null)
            {
                leftval = InterpretAST(ast.left);
            }
            if (ast.right != null)
            {
                rightval = InterpretAST(ast.right);
            }
            if (ast.op == ASTEnum.A_INTLIT)
            {
                Console.WriteLine($"int {ast.intvalue}");
            }
            else
            {
                Console.WriteLine($"{leftval} {tokstr[(int)ast.op]} {rightval} ");
            }
            switch (ast.op)
            {
                case ASTEnum.A_ADD:
                    return leftval + rightval;
                case ASTEnum.A_SUBTRACT:
                    return leftval - rightval;
                case ASTEnum.A_MULTIPLY:
                    return leftval * rightval;
                case ASTEnum.A_DIVIDE:
                    return leftval / rightval;
                case ASTEnum.A_INTLIT:
                    return ast.intvalue;
                default:
                    Console.WriteLine($"Unknown AST operator {ast.op}");
                    return 0;
            }
        }
    }
}
