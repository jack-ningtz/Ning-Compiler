using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler
{
    class CodeGeneration
    {
        static int[] freereg = { 1, 1, 1, 1 };
        static string[] reglist = { "%r8", "%r9", "%r10", "%r11" };
        static string[] breglist = { "%r8b", "%r9b", "%r10b", "%r11b" };

        public void Freeall_Registers()
        {
            freereg[0] = 1;
            freereg[1] = 1;
            freereg[2] = 1;
            freereg[3] = 1;
        }
        public static int Alloc_Register()
        {
            for (int i = 0; i < 4; i++)
            {
                if (freereg[i] != 0)
                {
                    freereg[i] = 0;
                    return i;
                }
            }
            Error.Fatal("Out of registers");
            return -1;
        }
        public void Free_Register(int reg)
        {
            if (freereg[reg] != 0)
            {
                Error.Fatal($"Error trying to free register {reg}");
            }
            freereg[reg] = 1;
        }
        //打印汇编前言
        public void CgPreamble()
        {
            Freeall_Registers();
            Io.WriteFile(
                "\t.text\n" +
                ".LC0:\n" +
                "\t.string\t\"%d\\n\"\n"+
                "printint:\n"+
                "\tpushq\t%rbp\n"+
                "\tmovq\t%rsp, %rbp\n"+
                "\tsubq\t$16, %rsp\n"+
                "\tmovl\t%edi, -4(%rbp)\n"+
                "\tmovl\t-4(%rbp), %eax\n"+
                "\tmovl\t%eax, %esi\n"+
                "\tleaq	.LC0(%rip), %rdi\n"+
                "\tmovl	$0, %eax\n"+
                "\tcall	printf@PLT\n"+
                "\tnop\n"+
                "\tleave\n"+
                "\tret\n"+
                "\n"+
                "\t.globl\tmain\n"+
                "\t.type\tmain, @function\n"+
                "main:\n"+
                "\tpushq\t%rbp\n"+
                "\tmovq	%rsp, %rbp\n"
                );
        }
        // 打印汇编后序
        public void CgPostamble()
        {
            Io.WriteExistsFile(
                "\tmovl	$0, %eax\n" +
                "\tpopq	%rbp\n" +
                "\tret\n" 
                );
        }
        public int CgLoadInt(int value)
        {
            int r = Alloc_Register();
            Io.WriteExistsFile($"\tmovq\t${value}, {reglist[r]}\n");
            return r;
        }
        public int CgLoadGlob(string identifier)
        {
            int r = Alloc_Register();
            Io.WriteExistsFile($"\tmovq\t{identifier}(%rip), {reglist[r]}\n");
            return r;
        }
        public int CgAdd(int r1, int r2)
        {
            Io.WriteExistsFile($"\taddq\t{reglist[r1]},{reglist[r2]}\n");
            Free_Register(r1);
            return r2;
        }
        public int CgSub(int r1, int r2)
        {
            Io.WriteExistsFile($"\tsubq\t{reglist[r2]}, {reglist[r1]}\n");
            Free_Register(r2);
            return r1;
        }
        public int CgMul(int r1, int r2)
        {
            Io.WriteExistsFile($"\timulq\t{reglist[r1]},{reglist[r2]}\n");
            Free_Register(r1);
            return r2;
        }
        public int CgDiv(int r1, int r2)
        {
            Io.WriteExistsFile($"\tmovq\t{reglist[r1]},%rax\n");
            Io.WriteExistsFile($"\tcqo\n");
            Io.WriteExistsFile($"\tidivq\t{reglist[r2]}\n");
            Io.WriteExistsFile($"\tmovq\t%rax,{reglist[r1]}\n");
            Free_Register(r2);
            return r1;
        }
        public void CgPrintint(int r)
        {
            Io.WriteExistsFile($"\tmovq\t{reglist[r]}, %rdi\n");
            Io.WriteExistsFile($"\tcall\tprintint\n");
            Free_Register(r);
        }
        //存储一个寄存器的值到变量中
        public int CgStorGlob(int r, string identifiter)
        {
            Io.WriteExistsFile($"\tmovq\t{reglist[r]}, {identifiter}(%rip)\n");
            return r;
        }
        public void CgGlobSym(string sym)
        {
            Io.WriteExistsFile($"\t.comm\t{sym},8,8\n");
        }
        //public int CgCompare(int r1, int r2, string how)
        //{
        //    Io.WriteExistsFile($"\tcmpq\t{reglist[r2]},{reglist[r1]}\n");
        //    Io.WriteExistsFile($"\t{how}\t{breglist[r2]}\n");
        //    Io.WriteExistsFile($"\tandq\t$255,{reglist[r2]}\n");
        //    Free_Register(r1);
        //    return r2;
        //}
        //// ==
        //public int CgEqual(int r1, int r2)
        //{
        //    return CgCompare(r1, r2, "sete");
        //}
        //// !=
        //public int CgNotEqual(int r1, int r2)
        //{
        //    return CgCompare(r1, r2, "setne");
        //}
        //// <
        //public int CgLessThan(int r1, int r2)
        //{
        //    return CgCompare(r1, r2, "setl");
        //}
        //// >
        //public int CgGreaterThan(int r1, int r2)
        //{
        //    return CgCompare(r1, r2,"setg");
        //}
        //// <=
        //public int CgLessEqual(int r1, int r2)
        //{
        //    return CgCompare(r1, r2, "setle");
        //}
        //// >=
        //public int CgGreaterEqual(int r1, int r2)
        //{
        //    return CgCompare(r1, r2, "setge");
        //}



        // 比较指令列表
        static string[] cmplist = { "sete", "setne","setl","setg","setle","setge"};
        public int CgCompare_And_Set(ASTEnum astop, int r1, int r2)
        {
            if ((astop < ASTEnum.A_EQ) || (astop > ASTEnum.A_GE))
            {
                Error.Fatal_General($"Bad ASTop in cgcompare_and_set()");
            }
            Io.WriteExistsFile($"\tcmpq\t{reglist[r2]},{reglist[r1]}\n");
            Io.WriteExistsFile($"\t{cmplist[astop - ASTEnum.A_EQ]}\t{breglist[r2]}\n");
            Io.WriteExistsFile($"\tmovzbq\t{breglist[r2]}, {reglist[r2]}\n");
            Free_Register(r1);
            return r2;
        }
        public void CgLabel(int l)
        {
            Io.WriteExistsFile($"L{l}:\n");
        }
        public void CgJump(int l)
        {
            Io.WriteExistsFile($"\tjmp\tL{l}\n");
        }
        // 跳转指令反向列表
        static string[] invcmplist = { "jne", "je", "jge", "jle", "jg", "jl" };
        public int CgCompare_And_Jump(ASTEnum astop, int r1, int r2, int label)
        {
            if ((astop < ASTEnum.A_EQ )||( astop > ASTEnum.A_GE))
            {
                Error.Fatal_General($"Bad ASTop in cgcompare_and_jump()");
            }
            Io.WriteExistsFile($"\tcmpq\t{reglist[r2]}, {reglist[r1]}\n");
            Io.WriteExistsFile($"\t{invcmplist[astop - ASTEnum.A_EQ]}\tL{label}\n");
            Freeall_Registers();
            return -1;
        }
    }
}
