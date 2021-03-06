using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler
{
    class AST
    {
        public ASTEnum op { get; set; }
        public AST left { get; set; }
        public AST right { get; set; }
        public int intvalue { get; set; }

        // AST node types

        public AST MkAst(ASTEnum op, AST left, AST right, int intvalue)
        {
            AST a = new AST();
            a.op = op;
            a.left = left;
            a.right = right;
            a.intvalue = intvalue;
            return a;
        }
        public AST MkAstLeaf(ASTEnum op, int intvalue)
        {
            return MkAst(op, null, null, intvalue);
        }
        public AST MkAstUnary(ASTEnum op, AST left, int intvalue)
        {
            return MkAst(op, left, null, intvalue);
        }
    }
    public enum ASTEnum
    {
        A_ADD, A_SUBTRACT, A_MULTIPLY, A_DIVIDE, A_INTLIT,A_ERROR
    };
}
