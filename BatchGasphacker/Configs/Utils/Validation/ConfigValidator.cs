using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BatchGasphacker.Configs.Utils.Validation
{
    public static class ConfigValidator
    {
        public static ConfigValidationResult CheckValidity(Config config)
        {
            var result = new ConfigValidationResult();
            var tasks = new List<Task>
            {
                CheckValidityOfInputDirectory(config, result),
                CheckValidityOfOutputDirectory(config, result),
                CheckValidityOfTtxExeDirectory(config, result),
                CheckValidityOfAfdkoDirectory(config, result)
            };

            Task.WaitAll(tasks.ToArray());
            return result;
        }

        private static async Task CheckValidityOfInputDirectory(Config config, ConfigValidationResult result)
        {
            await Task.Run(() =>
            {
                if (config.InputDirectory == null)
                {
                    result.InputDirectoryStatus = DirectoryStatus.NotSpecified();
                    return;
                }

                if (!Directory.Exists(config.InputDirectory))
                {
                    result.InputDirectoryStatus = DirectoryStatus.Invalid();
                    return;
                }

                result.TtfFiles = Directory
                    .EnumerateFiles(config.InputDirectory, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(it => it.ToLowerInvariant().EndsWith(".ttf")).Select(it => new FileInfo(it));
                result.TtcFiles = Directory
                    .EnumerateFiles(config.InputDirectory, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(it => it.ToLowerInvariant().EndsWith(".ttc")).Select(it => new FileInfo(it));
                if (!result.TtfFiles.Any() && !result.TtcFiles.Any())
                {
                    result.InputDirectoryStatus = DirectoryStatus.Empty();
                    return;
                }

                result.InputDirectoryStatus = DirectoryStatus.Ok();
            });
        }

        private static async Task CheckValidityOfOutputDirectory(Config config, ConfigValidationResult result)
        {
            await Task.Run(() =>
            {
                if (config.OutputDirectory == null)
                {
                    result.OutputDirectoryStatus = DirectoryStatus.NotSpecified();
                    return;
                }

                if (!Directory.Exists(config.OutputDirectory))
                {
                    result.OutputDirectoryStatus = DirectoryStatus.Invalid();
                    return;
                }

                result.OutputDirectoryStatus = DirectoryStatus.Ok();
            });
        }

        private static async Task CheckValidityOfTtxExeDirectory(Config config, ConfigValidationResult result)
        {
            await Task.Run(() =>
            {
                if (config.TtxExeDirectory == null)
                {
                    result.TtxExeDirectoryStatus = DirectoryStatus.NotSpecified();
                    return;
                }

                if (!Directory.Exists(config.TtxExeDirectory))
                {
                    result.TtxExeDirectoryStatus = DirectoryStatus.Invalid();
                    return;
                }

                var filesNotExisting = new List<string>();
                if (!File.Exists(config.TtxExeDirectory + "/ttx.exe"))
                    filesNotExisting.Add("ttx.exe");
                if (!File.Exists(config.TtxExeDirectory + "/GaspHack.ttx"))
                    filesNotExisting.Add("GaspHack.ttx");

                if (filesNotExisting.Any())
                {
                    result.TtxExeDirectoryStatus = DirectoryStatus.FileNotFound(filesNotExisting.ToArray());
                    return;
                }

                result.TtxExeDirectoryStatus = DirectoryStatus.Ok();
            });
        }

        private static async Task CheckValidityOfAfdkoDirectory(Config config, ConfigValidationResult result)
        {
            await Task.Run(() =>
            {
                if (config.AfdkoDirectory == null)
                {
                    result.AfdkoDirectoryStatus = DirectoryStatus.NotSpecified();
                    return;
                }

                if (!Directory.Exists(config.AfdkoDirectory))
                {
                    result.AfdkoDirectoryStatus = DirectoryStatus.Invalid();
                    return;
                }

                var filesNotExisting = new List<string>();
                if (!File.Exists(config.AfdkoDirectory + "/otf2otc.cmd"))
                    filesNotExisting.Add("otf2otc.cmd");
                if (!File.Exists(config.AfdkoDirectory + "/otc2otf.cmd"))
                    filesNotExisting.Add("otc2otf.cmd");

                if (filesNotExisting.Any())
                    result.AfdkoDirectoryStatus = DirectoryStatus.FileNotFound(filesNotExisting.ToArray());
                else
                    result.AfdkoDirectoryStatus = DirectoryStatus.Ok();
            });
        }
    }
}
