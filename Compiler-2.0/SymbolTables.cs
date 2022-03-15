using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler
{
    class Symt
    {
        public Symt(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
    }
    class SymbolTables
    {

        // 下一个空闲符号的位置
        public static int GlobSymPos = 0;
        public static Symt[] symts = new Symt[1024];

        // Symbol name
        public int FindGlob(string s)
        {
            int i;
            for (i = 0; i < GlobSymPos; i++)
            {
                if (s == symts[i].Name)
                    return i;
            }
            return -1;
        }
        public int NewGlob(string name)
        {
            int i = 0;
            i = GlobSymPos++;
            if (i >= 1024)
                Error.Fatal("Too many global symbols");
            symts[i] = new Symt(name);
            return i;
        }
        public int AddGlob(string name)
        {
            int y;
            y = FindGlob(name);
            if(y != (-1))
            {
                return y;
            }
            y = NewGlob(name);
            return y;

        }
    }
}
