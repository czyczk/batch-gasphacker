using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace BatchGasphacker.Utils
{
    public static class CmdExecutor
    {
        public static int ExecuteCommand(string command, bool debug = false)
        {

            var processInfo = new ProcessStartInfo("cmd.exe", "/c" + command)
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };

            if (debug)
            {
                processInfo.RedirectStandardError = true;
                processInfo.RedirectStandardOutput = true;
            }

            var process = Process.Start(processInfo);

            if (debug)
            {
                process.OutputDataReceived += (sender, e) => { Console.WriteLine($"Output: {e.Data}"); };
                process.BeginOutputReadLine();

                process.ErrorDataReceived += (sender, e) => { Console.WriteLine($"Error: {e.Data}"); };
                process.BeginErrorReadLine();
            }

            process.WaitForExit();

            var exitCode = process.ExitCode;
            if (debug)
                Console.WriteLine($"Exit code: {exitCode}");

            process.Close();
            return exitCode;
        }
    }
}
