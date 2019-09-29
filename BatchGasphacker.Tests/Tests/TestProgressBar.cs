using System;
using System.Collections.Generic;
using System.Text;
using BatchGasphacker.Configs.Utils.ProgressBar;

namespace BatchGasphacker.Tests.Tests
{
    public static class TestProgressBar
    {
        public static void Run()
        {
            // Experiment
            var pb = new ProgressBar(3, 2);
            Console.ReadKey();
            pb.StartProcessingTtf();
            Console.ReadKey();
            pb.StartProcessingTtf("1.ttf");
            Console.ReadKey();
            pb.DoneProcessingTtf("1.ttf");
            Console.ReadKey();
            pb.StartProcessingTtf("2.ttf");
            Console.ReadKey();
            pb.DoneProcessingTtf("2.ttf");
            Console.ReadKey();
            pb.StartProcessingTtf("3.ttf");
            Console.ReadKey();
            pb.DoneProcessingTtf("3.ttf");
            Console.ReadKey();
            pb.DoneProcessingTtf();
            Console.ReadKey();
            pb.StartProcessingTtc();
            Console.ReadKey();
            pb.StartProcessingTtc("msyh.ttc");
            Console.ReadKey();
            pb.DoneProcessingTtc("msyh.ttc");
            Console.ReadKey();
            pb.StartProcessingTtc("bbbb.ttc");
            Console.ReadKey();
            pb.DoneProcessingTtc("bbbb.ttc");
            Console.ReadKey();
            pb.DoneProcessingTtc();
            Console.ReadKey();
            pb.MarkDone();
            pb.Dispose();
        }
    }
}
