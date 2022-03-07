using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler
{
    class Gen
    {
        private static CodeGeneration generation = new CodeGeneration();
        public static int GenAST(AST n , int reg)
        {
            int leftreg=0, rightreg=0;
            if (n.left != null)
                leftreg = Gen.GenAST(n.left, -1);
            if (n.right != null)
                rightreg = Gen.GenAST(n.right, leftreg);
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
                case ASTEnum.A_EQ:
                    return generation.CgEqual(leftreg, rightreg);
                case ASTEnum.A_NE:
                    return generation.CgNotEqual(leftreg, rightreg);
                case ASTEnum.A_LT:
                    return generation.CgLessThan(leftreg, rightreg);
                case ASTEnum.A_GT:
                    return generation.CgGreaterThan(leftreg, rightreg);
                case ASTEnum.A_LE:
                    return generation.CgLessEqual(leftreg, rightreg);
                case ASTEnum.A_GE:
                    return generation.CgGreaterEqual(leftreg, rightreg);
                case ASTEnum.A_INTLIT:
                    return generation.CgLoadInt(n.intvalue);
                case ASTEnum.A_IDENT:
                    return generation.CgLoadGlob(SymbolTables.symts[n.intvalue].Name);
                case ASTEnum.A_LVIDENT:
                    return generation.CgStorGlob(reg, SymbolTables.symts[n.intvalue].Name);
                case ASTEnum.A_ASSIGN:
                    return rightreg;
                default:
                    Error.Fatal($"Unknown AST operator {n.op}");
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
        public static void GenGlobSym(string name)
        {
            generation.CgGlobSym(name);
        }
    }
}
