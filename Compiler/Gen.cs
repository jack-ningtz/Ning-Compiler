using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler
{
    class Gen
    {
        private static CodeGeneration generation = new CodeGeneration();
        public static int GenAST(AST n)
        {
            int leftreg=0, rightreg=0;
            if (n.left != null)
                leftreg = Gen.GenAST(n.left);
            if (n.right != null)
                rightreg = Gen.GenAST(n.right);
            switch (n.op)
            {
                case ASTEnum.A_ADD:
                    return generation.CgAdd(leftreg, rightreg);
                case ASTEnum.A_SUBTRACT:
                    return generation.CgSub(leftreg, rightreg);
                case ASTEnum.A_MULTIPLY:
                    return generation.CgMul(leftreg, rightreg);
                case ASTEnum.A_DIVIDE:
                    return generation.CgDiv(leftreg, rightreg);
                case ASTEnum.A_INTLIT:
                    return generation.CgLoad(n.intvalue);
                default:
                    Console.WriteLine($"Unknown AST operator {n.op}");
                    Environment.Exit(0);
                    return -1;
            }
        }
        //public static void Generatecode(AST n)
        //{
        //    int reg;
        //    generation.CgPreamble();
        //    reg = GenAST(n);
        //    generation.CgPostamble();
        //}
        public static void GenPreamble()
        {
            generation.CgPreamble();
        }
        public static void GenPostamble()
        {
            generation.CgPostamble();
        }
        public static void GenFreeregs()
        {
            generation.Freeall_Registers();
        }
        public static void GenPrintInt(int reg)
        {
            generation.CgPrintint(reg);
        }
    }
}
