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
                    t.token = Enum.T_START;
                    break;
                case '/':
                    t.token = Enum.T_SLASH;
                    break;
                default:
                    if (Char.IsDigit(c))
                    {
                        t.token = Enum.T_INTLIT;
                        t.value = ScanInt(c);
                        break;
                    }
                    Console.WriteLine("Unrecognised character {0} on line {1}\n",  c, line);
                    return t;
            }
            return t;
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
