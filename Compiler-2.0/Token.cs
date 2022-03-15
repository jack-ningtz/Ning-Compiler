using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler
{
    // Token class
    public class Token
    {
        public Enum token { get; set; }
        public int value { get; set; }
    }
    public enum Enum
    {
        T_EOF,
        T_PLUS,
        T_MINUS,
        T_STAR,
        T_SLASH,

        T_EQ, T_NE,
        T_LT, T_GT, T_LE, T_GE,

        T_INTLIT,
        T_SEMI,

        T_ASSIGN,
        //T_EQUALS,
        T_IDENT,

        T_LBRACE, T_RBRACE, T_LPAREN, T_RPAREN,
        // Keywords
        T_PRINT,
        T_INT,
        T_IF, T_ELSE,T_WHILE, T_FOR

    }

}
