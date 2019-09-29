using System;
using System.Collections.Generic;
using System.Text;
using BatchGasphacker.Utils;

namespace BatchGasphacker.Tests.Tests
{
    static class TestCmdExecutor
    {
        public static void Run()
        {
            CmdExecutor.ExecuteCommand("D: & mkdir aaa", true);
        }
    }
}
