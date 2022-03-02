using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Compiler
{
    class Io
    {
        static FileStream fs;
        static FileStream writefs;
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
        public static void WriteFile(string s)
        {
            try
            {
                if (File.Exists("out.s"))
                {
                    File.Delete("out.s");  
                }
                using (writefs = File.Create("out.s"))
                {
                    AddText(writefs, s);
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }
        }
        private static void AddText(FileStream fs, string s)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(s);
            fs.Write(info, 0, info.Length);
        }
        public  static void WriteExistsFile(string s)
        {
            byte[] result = new UTF8Encoding().GetBytes(s);
            using (writefs = File.Open("out.s", FileMode.OpenOrCreate))
            {
                writefs.Seek(0, SeekOrigin.End);
                writefs.Write(result, 0, result.Length);
            }
        }
    }
}
