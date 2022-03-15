using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler
{
    class Error
    {
        public static int l = Lexer.line;
        public static void Fatal(string s)
        {
            Console.WriteLine(s);
            Environment.Exit(0);
        }
        public static void Fatal_General(string s)
        {
            Console.WriteLine($"{s} on line {l}\n");
            Environment.Exit(0);
        }
        public static void Fatal(string s, int line)
        {
            Console.WriteLine($"{s} on line {line}\n");
            Environment.Exit(0);
        }
        public static void Fatals(string s1, string s2, int line)
        {
            Console.WriteLine($"{s1}:{s2} on line {line}\n");
            Environment.Exit(0);
        }
        public static void Fatald(string s, Enum d, int line)
        {
            Console.WriteLine($"{s}:{d} on line {line}\n");
            Environment.Exit(0);
        }
        public static void Fatald(string s, ASTEnum d, int line)
        {
            Console.WriteLine($"{s}:{d} on line {line}\n");
            Environment.Exit(0);
        }
        public static void Fatalc(string s, char c, int line)
        {
            Console.WriteLine($"{s}:{s} on line {line}\n");
            Environment.Exit(0);
        }

    }
}
