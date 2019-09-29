using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BatchGasphacker.Tests.Tests
{
    static class TestCSharpFeature
    {
        public static void Run()
        {
            //var file = new FileInfo(@"D:\Users\Zenas\Music\Electro Art\Ujicox\Ujicox - Remembrance.mpc");
            //var file = new FileInfo(@"D:\a.txt");
            //Console.WriteLine(file.DirectoryName);
            //Console.WriteLine(file.Name);
            //Console.WriteLine(file.FullName);
//            Console.WriteLine(Path.Combine("D:\\", "temp"));
            File.WriteAllLines("test.log", new [] {"a", "b", "c"});
        }
    }
}
