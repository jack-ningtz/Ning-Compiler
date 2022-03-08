using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Compiler
{
    class Lexer
    {
        private FileStream stream; 
        public static int line = 1;
        private char putback = '\0';
        private readonly int TEXTLEN = 512;
        //标识符或关键字
        public static string TEXT;
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
                case '{':
                    t.token = Enum.T_LBRACE;
                    break;
                case '}':
                    t.token = Enum.T_RBRACE;
                    break;
                case '(':
                    t.token = Enum.T_LPAREN;
                    break;
                case ')':
                    t.token = Enum.T_RPAREN;
                    break;
                case '=':
                    if ((c = NextChar()) == '=')
                    {
                        t.token = Enum.T_EQ; // ==
                    }
                    else
                    {
                        Putback(c);
                        t.token = Enum.T_ASSIGN; // =
                    }
                    break;
                case '!':
                    if ((c = NextChar()) == '=')
                    {
                        t.token = Enum.T_NE; // !=
                    }
                    else
                    {
                        Error.Fatalc("Unrecognised character", c, line);
                    }
                    break;
                case '<':
                    if ((c = NextChar()) == '=')
                    {
                        t.token = Enum.T_LE; // <=
                    }
                    else
                    {
                        Putback(c);
                        t.token = Enum.T_LT; // <
                    }
                    break;
                case '>':
                    if ((c = NextChar()) == '=')
                    {
                        t.token = Enum.T_GE; // >=
                    }
                    else 
                    {
                        Putback(c);
                        t.token = Enum.T_GT; // >
                    }
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
                        if (tokentype != 0)
                        {
                            t.token = tokentype;
                            break;
                        }
                        t.token = Enum.T_IDENT;
                        break;
                    }
                    Error.Fatalc("Unrecognised character", c, line);
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
            if (tEXT.Equals("int")) 
            {
                return Enum.T_INT;
            }
            if (tEXT.Equals("if"))
            {
                return Enum.T_IF;
            }
            if (tEXT.Equals("else"))
            {
                return Enum.T_ELSE;
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
                    Error.Fatal("identifier too long on line", line);
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
