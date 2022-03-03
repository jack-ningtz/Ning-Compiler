using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler
{
    class CodeGeneration
    {
        static int[] freereg = { 1, 1, 1, 1 };
        static string[] reglist = { "%r8", "%r9", "%r10", "%r11" };

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
            Console.WriteLine("Out of registers");
            Environment.Exit(0);
            return -1;
        }
        public void Free_Register(int reg)
        {
            if (freereg[reg] != 0)
            {
                Console.WriteLine($"Error trying to free register {reg}");
                Environment.Exit(0);
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
        public int CgLoad(int value)
        {
            int r = Alloc_Register();
            Io.WriteExistsFile($"\tmovq\t${value}, {reglist[r]}\n");
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
    }
}
