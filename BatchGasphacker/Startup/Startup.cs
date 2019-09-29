using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using BatchGasphacker.Configs;
using BatchGasphacker.Configs.Utils;
using BatchGasphacker.Configs.Utils.Validation;
using BatchGasphacker.Utils;
using ProgressBar = BatchGasphacker.Configs.Utils.ProgressBar.ProgressBar;

namespace BatchGasphacker.Startup
{
    public class Startup
    {
        public void Run()
        {
            // Clean the previous failure log
            if (File.Exists("failures.log"))
            {
                File.Delete("failures.log");
            }

            Config config;

            // Load config from the file
            try
            {
                config = ConfigLoader.LoadConfig(ConfigLoader.DefaultFilename);
            }
            catch (FileNotFoundException)
            {
                Console.Error.WriteLine($"Configuration file \"{ConfigLoader.DefaultFilename}\" not found.");
                Console.ReadLine();
                return;
            }

            // Check validity of the config
            var configValidationResult = ConfigValidator.CheckValidity(config);
            var configProblems = configValidationResult.ReportProblems();
            if (configProblems.Any())
            {
                Console.Error.WriteLine($"{configProblems.Count} problem(s) detected in the configuration file.");
                configProblems.ForEach(it =>
                {
                    Console.Error.WriteLine($"- {it}");
                });
                Console.ReadLine();
                return;
            }

            // Initiate a progress bar
            var pb = new ProgressBar(configValidationResult.TtfFiles.Count(), configValidationResult.TtcFiles.Count());

            // Process TTF fonts
            var failedTtfFonts = ProcessTtfFonts(config, configValidationResult.TtfFiles.ToList(), pb);

            // Process TTC fonts
            var failedTtcFonts = ProcessTtcFonts(config, configValidationResult.TtcFiles.ToList(), pb);

            // Done processing
            pb.MarkDone();
            pb.Dispose();

            if (failedTtfFonts.Any() || failedTtcFonts.Any())
            {
                // Write the failures to "failures.log"
                File.WriteAllLines("failures.log", failedTtfFonts.Concat(failedTtcFonts).Select(it => it.Name));

                // Report
                Console.WriteLine($"Completed with the following {failedTtfFonts.Count + failedTtcFonts.Count} failures. They have been written in \"failures.log\".");
                foreach (var failure in failedTtfFonts.Concat(failedTtcFonts))
                {
                    Console.WriteLine($"- {failure.Name}");
                }
            }
            Console.ReadLine();
        }

        private IList<FileInfo> ProcessTtfFonts(Config config, IList<FileInfo> fonts, ProgressBar pb)
        {
            var failureList = new List<FileInfo>();

            if (!fonts.Any())
                return failureList;

            pb.StartProcessingTtf();

            foreach (var font in fonts)
            {
                pb.StartProcessingTtf(font.Name);
                // X: & cd "{TtxExeDirectory}" & ttx.exe -o "{OutputDirectory}\{fontName}" -m "{InputDirectory}\{fontName}" GaspHack.ttx
                var exitCode = CmdExecutor.ExecuteCommand(
                    $"{config.TtxExeDirectory.Substring(0, 2)} & cd \"{config.TtxExeDirectory}\" & ttx.exe -o \"{Path.Combine(config.OutputDirectory, font.Name)}\" -m \"{font.FullName}\" GaspHack.ttx");
                if (exitCode != 0)
                    failureList.Add(font);

                pb.DoneProcessingTtf(font.Name);
            }

            // # {InputDirectory}\*.ttf should == # {OutputDirectory}\*.ttf + # failure fonts
            Debug.Assert(fonts.Count ==
                         Directory.EnumerateFiles(config.OutputDirectory, "*.*").Count(it => it.ToLowerInvariant().EndsWith(".ttf")) +
                         failureList.Count);

            pb.DoneProcessingTtf();

            return failureList;
        }

        private IList<FileInfo> ProcessTtcFonts(Config config, IList<FileInfo> fonts, ProgressBar pb)
        {
            var failureList = new List<FileInfo>();

            if (!fonts.Any())
                return failureList;

            var tempInputDirectory = Path.Combine(config.InputDirectory, "temp");
            if (!Directory.Exists(tempInputDirectory))
            {
                // Create a temp folder in the input folder
                Directory.CreateDirectory(tempInputDirectory);
            }
            else
            {
                // Clean the temp files
                DirectoryCleaner.CleanDirectoryAsync(tempInputDirectory).Wait();
            }

            // Create a temp folder in the output folder
            var tempOutputDirectory = Path.Combine(config.OutputDirectory, "temp");
            if (!Directory.Exists(tempOutputDirectory))
            {
                // Create a temp folder in the input folder
                Directory.CreateDirectory(tempOutputDirectory);
            }
            else
            {
                // Clean the temp files
                DirectoryCleaner.CleanDirectoryAsync(tempOutputDirectory).Wait();
            }

            pb.StartProcessingTtc();

            foreach (var font in fonts)
            {
                pb.StartProcessingTtc(font.Name);

                // X: & cd {InputDirectory}\temp & call "{AfdkoDirectory}\otc2otf.cmd" "{fontFullname}"
                pb.StartExtractingTtc(font.Name);
                var exitCode =
                    CmdExecutor.ExecuteCommand(
                        $"{tempInputDirectory.Substring(0, 2)} & cd \"{tempInputDirectory}\" & call \"{Path.Combine(config.AfdkoDirectory, "otc2otf.cmd")}\" \"{font.FullName}\"");

                Debug.Assert(exitCode == 0); // TODO
                if (exitCode != 0)
                {
                    failureList.Add(font);
                }
                else
                {
                    pb.StartProcessingExtractedFilesOf(font.Name);
                    var extractedTtfFiles = Directory.GetFiles(tempInputDirectory);
                    Debug.Assert(extractedTtfFiles.Any());
                    var exitCode2 = 0;
                    // Gasphack the extracted TTF files. Note that they should be processed in order of creation time to maintain the same order as the components in the original TTC file.
                    foreach (var extractedTtf in extractedTtfFiles.Select(it => new FileInfo(it)).OrderBy(it => it.CreationTimeUtc))
                    {
                        // X: & cd "{TtxExeDirectory}" & ttx.exe -o "{OutputDirectory}\temp\{fontName}" -m "{InputDirectory}\temp\{fontName}" GaspHack.ttx
                        exitCode2 = CmdExecutor.ExecuteCommand(
                            $"{config.TtxExeDirectory.Substring(0, 2)} & cd \"{config.TtxExeDirectory}\" & ttx.exe -o \"{Path.Combine(tempOutputDirectory, extractedTtf.Name)}\" -m \"{extractedTtf.FullName}\" GaspHack.ttx");
                        Debug.Assert(exitCode2 == 0); // TODO
                        if (exitCode2 != 0)
                            break;
                    }

                    if (exitCode2 != 0)
                    {
                        failureList.Add(font);
                    }
                    else
                    {
                        Debug.Assert(Directory.EnumerateFiles(tempOutputDirectory).Any());
                        pb.StartBuildingProcessedTtc(font.Name);
                        // X: & cd "{OutputDirectory}" & call "{AfdkoDirectory}\otf2otc.cmd" -o "{OutputDirectory}\{fontName}" "{OutputDirectory}\temp\1.ttf" "{OutputDirectory}\temp\2.ttf" ...
                        // Note that the input files should be ordered be creation time to maintain the same order as the components in the original TTC file.
                        var command = $"{config.OutputDirectory.Substring(0, 2)} & cd \"{config.OutputDirectory}\" & call \"{Path.Combine(config.AfdkoDirectory, "otf2otc.cmd")}\" -o \"{Path.Combine(config.OutputDirectory, font.Name)}\" ";
                        command = Directory.EnumerateFiles(tempOutputDirectory).Select(it => new FileInfo(it)).OrderBy(it => it.CreationTimeUtc)
                            .Aggregate(command,
                                (current, processedExtractedTtf) => current + $"\"{processedExtractedTtf.FullName}\" ");

                        var exitCode3 = CmdExecutor.ExecuteCommand(command);
                        Debug.Assert(exitCode3 == 0); // TODO
                        if (exitCode3 != 0)
                        {
                            failureList.Add(font);
                        }
                    }
                }

                // Clean the temp files
                Task.WaitAll(
                    DirectoryCleaner.CleanDirectoryAsync(tempInputDirectory),
                    DirectoryCleaner.CleanDirectoryAsync(tempOutputDirectory)
                    );
                pb.DoneProcessingTtc(font.Name);
            }

            // Delete the temp folders
            Debug.Assert(!Directory.EnumerateFiles(tempInputDirectory).Any());
            Debug.Assert(!Directory.EnumerateFiles(tempOutputDirectory).Any());
            try
            {
                Directory.Delete(tempInputDirectory);
                Directory.Delete(tempOutputDirectory);
            }
            catch (IOException)
            {
                // Doesn't matter if the temp folder is occupied.
            }

            // # {InputDirectory}\*.ttc should == # {OutputDirectory}\*.ttc + # failure fonts
            Debug.Assert(fonts.Count ==
                         Directory.EnumerateFiles(config.OutputDirectory, "*.*").Count(it => it.ToLowerInvariant().EndsWith(".ttc")) +
                         failureList.Count);

            pb.DoneProcessingTtc();

            return failureList;
        }
    }
}
