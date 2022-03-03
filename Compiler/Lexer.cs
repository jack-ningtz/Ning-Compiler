using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Compiler
{
    class Lexer
    {
        private FileStream stream; 
        public int line = 1;
        private char putback = '\0';
        private readonly int TEXTLEN = 512;
        //标识符或关键字
        private string TEXT;
        public Lexer(FileStream stream)
        {
            this.stream = stream;
        }
        // 放回一个字符
        public void Putback(char c)
        {
            putback = c;
        }
        // 忽略空白行
        public char SkipWhiteSpace()
        {
            char c = NextChar();
            while ( ' ' == c || '\t' == c || '\n' == c || '\r' == c || '\f' == c)
            {
                c = NextChar();
            }
            return c;
        }
        // 读入下一个字符
        public char NextChar()
        {
            if (putback != '\0')
            {
                char ch = putback;
                putback = '\0';
                return ch;
            }
            char c = (char)stream.ReadByte();
            if ('\n' == c)
                line++;
            return c;
        }
        public Token Scan()
        {
            Token t = new Token();
            t.token = Enum.T_EOF;
            Enum tokentype = 0;
            char c = SkipWhiteSpace();
            // 最后字符
            if (Convert.ToInt32(c).Equals(65535))
            {
                return t;
            }
            switch (c)
            {
                case '+':
                    t.token = Enum.T_PLUS;
                    break;
                case '-':
                    t.token = Enum.T_MINUS;
                    break;
                case '*':
                    t.token = Enum.T_STAR;
                    break;
                case '/':
                    t.token = Enum.T_SLASH;
                    break;
                case ';':
                    t.token = Enum.T_SEMI;
                    break;
                default:
                    if (Char.IsDigit(c))
                    {
                        t.token = Enum.T_INTLIT;
                        t.value = ScanInt(c);
                        break;
                    }
                    else if (Char.IsLetter(c) || c == '_')
                    {
                        TEXT = ScanIdent(c, TEXTLEN);
                        tokentype = KeyWord(TEXT);
                        if (tokentype == Enum.T_PRINT)
                        {
                            t.token = tokentype;
                            break;
                        }
                        Console.WriteLine("Unrecognised symbol {0} on line {1}\n", TEXT, line);
                        Environment.Exit(0);
                    }
                    Console.WriteLine("Unrecognised character {0} on line {1}\n",  c, line);
                    Environment.Exit(0);
                    return t;
            }
            return t;
        }

        private Enum KeyWord(string tEXT)
        {
            if (tEXT.Equals("print"))
            {
                return Enum.T_PRINT;
            }
            return 0;
        }

        private string ScanIdent(char c, int tEXTLEN)
        {
            string str = "";
            int i = 0;
            while (char.IsDigit(c) || char.IsLetter(c) || '_' == c)
            {
                if (tEXTLEN - 1 == i)
                {
                    Console.WriteLine($"identifier too long on line {line}\n");
                    Environment.Exit(1);
                }
                else if(i < (tEXTLEN - 1))
                {
                    str += c.ToString();
                    i++;
                }
                c = NextChar();
            }
            Putback(c);
            return str;
        }

        private int ScanInt(char c)
        {
            string str = "";
            str += c.ToString();
            char v = NextChar();
            while (Char.IsDigit(v))
            {
                str += v.ToString();
                v = NextChar();
            }
            Putback(v);
            return Convert.ToInt32(str);
        }
    }
}
