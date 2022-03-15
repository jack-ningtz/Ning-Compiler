using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler
{
    class Gen
    {
        private static CodeGeneration generation = new CodeGeneration();
        public static int NOREG = -1;
        public static int id = 1;
        public static int Label()
        {
            return id++;
        }

        public static int GenIFAST(AST n)
        {
            int Lfalse = 0, Lend = 0;
            Lfalse = Label();
            if (n.right != null)
                Lend = Label();
            GenAST(n.left, Lfalse, n.op);
            GenFreeregs();
            GenAST(n.mid, NOREG, n.op);
            GenFreeregs();
            if (n.right != null)
                generation.CgJump(Lend);
            generation.CgLabel(Lfalse);
            if (n.right != null)
            {
                GenAST(n.right, NOREG, n.op);
                GenFreeregs();
                generation.CgLabel(Lend);
            }
            return NOREG;
        }
        public static int GenWhileAST(AST n)
        {
            int Lstart = 0, Lend = 0;
            Lstart = Label();
            Lend = Label();
            generation.CgLabel(Lstart);
            GenAST(n.left, Lend, n.op);
            GenFreeregs();
            GenAST(n.right, NOREG, n.op);
            GenFreeregs();

  
            generation.CgJump(Lstart);
            generation.CgLabel(Lend);
            return -1;
        }
        public static int GenAST(AST n, int reg, ASTEnum parentASTop)
        {
            int leftreg = 0, rightreg = 0;
            switch (n.op)
            {
                case ASTEnum.A_IF:
                    return GenIFAST(n);
                case ASTEnum.A_WHILE:
                    return GenWhileAST(n);
                case ASTEnum.A_GLUE:
                    GenAST(n.left, NOREG, n.op);
                    GenFreeregs();
                    GenAST(n.right, NOREG, n.op);
                    GenFreeregs();
                    return NOREG;
            }

            if (n.left != null)
                leftreg = GenAST(n.left, NOREG, n.op);
            if (n.right != null)
                rightreg = GenAST(n.right, leftreg, n.op);
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
                case ASTEnum.A_NE:
                case ASTEnum.A_LT:
                case ASTEnum.A_GT:
                case ASTEnum.A_LE:
                case ASTEnum.A_GE:
                    if (parentASTop == ASTEnum.A_IF || parentASTop == ASTEnum.A_WHILE)
                    {
                        return generation.CgCompare_And_Jump(n.op, leftreg, rightreg, reg);
                    }
                    else
                    {
                        return generation.CgCompare_And_Set(n.op, leftreg, rightreg);
                    }
                case ASTEnum.A_INTLIT:
                    return generation.CgLoadInt(n.intvalue);
                case ASTEnum.A_IDENT:
                    return generation.CgLoadGlob(SymbolTables.symts[n.intvalue].Name);
                case ASTEnum.A_LVIDENT:
                    return generation.CgStorGlob(reg, SymbolTables.symts[n.intvalue].Name);
                case ASTEnum.A_ASSIGN:
                    return rightreg;
                case ASTEnum.A_PRINT:
                    GenPrintInt(leftreg);
                    GenFreeregs();
                    return NOREG;
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
