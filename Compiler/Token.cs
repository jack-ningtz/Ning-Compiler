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
        T_INTLIT,
        T_SEMI, 

        T_EQUALS,
        T_IDENT,
        // Keywords
        T_PRINT,
        T_INT
    }

}
