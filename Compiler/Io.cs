using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Compiler
{
    class Io
    {
        static FileStream fs;

        public static FileStream ReadFile(string s)
        {
            try
            {
                fs = File.OpenRead(s);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e);
            }
            return fs;
        }
        
    }
}
