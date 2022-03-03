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
            Parser2 parser = new Parser2(token,lexer);
            Statement statement = new Statement(parser);

            Gen.GenPreamble();
            statement.Statements();
            Gen.GenPostamble();

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
                    Environment.Exit(0);
                    return 0;
            }
        }
    }
}
