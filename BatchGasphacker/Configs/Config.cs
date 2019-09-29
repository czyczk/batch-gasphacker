using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BatchGasphacker.Configs
{
    public class Config
    {
        public string InputDirectory { get; set; }

        public string OutputDirectory { get; set; }

        public string TtxExeDirectory { get; set; }

        public string AfdkoDirectory { get; set; }
    }
}
