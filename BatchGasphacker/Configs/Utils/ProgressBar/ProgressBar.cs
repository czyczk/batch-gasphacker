using System;
using System.Collections.Generic;
using System.Text;
using ShellProgressBar;

namespace BatchGasphacker.Configs.Utils.ProgressBar
{
    public class ProgressBar : IDisposable
    {
        public int NumTtf { get; protected set; }

        public int NumTtc { get; protected set; }

        private ShellProgressBar.ProgressBar OverallProgressBar { get; set; }

        private ChildProgressBar TtfProgressBar { get; set; }

        private ChildProgressBar TtcProgressBar { get; set; }

        private static readonly ProgressBarOptions OverallProgressBarOptions = new ProgressBarOptions
        {
            ForegroundColor = ConsoleColor.Yellow,
            BackgroundColor = ConsoleColor.DarkYellow,
            ProgressCharacter = '─',
            EnableTaskBarProgress = true
        };

        private static readonly ProgressBarOptions ChildProgressBarOptions = new ProgressBarOptions()
        {
            ProgressCharacter = '-',
            CollapseWhenFinished = false,
        };

        #region TTF files processing
        /// <summary>
        /// Called when it starts processing TTF files.
        /// </summary>
        public void StartProcessingTtf()
        {
            TtfProgressBar = OverallProgressBar.Spawn(NumTtf, null, ChildProgressBarOptions);
            OverallProgressBar.Message = $"[{OverallProgressBar.CurrentTick} / {OverallProgressBar.MaxTicks}] Processing TTF files...";
        }

        /// <summary>
        /// Called when it starts moving TTF files.
        /// Should be called twice. Once from {InputDirectory} to {TtxExeDirectory}, and the other from {TtxExeDirectory}/output to {OutputDirectory}.
        /// </summary>
        public void StartMovingTtf() =>
            TtfProgressBar.Message = "Moving TTF files...";

        /// <summary>
        /// Called when it starts to process a TTF file with the specified filename.
        /// </summary>
        /// <param name="filename">The filename of the font.</param>
        public void StartProcessingTtf(string filename) => TtfProgressBar.Message = $"[{TtfProgressBar.CurrentTick} / {TtfProgressBar.MaxTicks}] \"{filename}\": Processing...";

        /// <summary>
        /// Called after all TTF files are processed and the output files have been moved to {OutputDirectory}.
        /// </summary>
        public void DoneProcessingTtf()
        {
            TtfProgressBar.Message = $"[{TtfProgressBar.CurrentTick} / {TtfProgressBar.MaxTicks}] Done processing TTF files.";
            OverallProgressBar.Message = $"[{OverallProgressBar.CurrentTick} / {OverallProgressBar.MaxTicks}] Done processing TTF files.";
        }

        /// <summary>
        /// Called after the TTF file with the specified filename is done processing.
        /// </summary>
        /// <param name="filename">The filename of the font.</param>
        public void DoneProcessingTtf(string filename)
        {
            TtfProgressBar.Tick($"[{TtfProgressBar.CurrentTick + 1} / {TtfProgressBar.MaxTicks}] \"{filename}\": Done processing.");
            OverallProgressBar.Tick($"[{OverallProgressBar.CurrentTick + 1} / {OverallProgressBar.MaxTicks}] Processing TTF files...");
        }
        #endregion

        #region TTC files processing
        /// <summary>
        /// Called when it starts processing TTC files.
        /// </summary>
        public void StartProcessingTtc()
        {
            TtcProgressBar = OverallProgressBar.Spawn(NumTtc, null, ChildProgressBarOptions);
            OverallProgressBar.Message = $"[{OverallProgressBar.CurrentTick} / {OverallProgressBar.MaxTicks}] Processing TTC files...";
        }

        /// <summary>
        /// Called when it starts to process a TTC file with the specified filename.
        /// </summary>
        /// <param name="filename">The filename of the font.</param>
        public void StartProcessingTtc(string filename) => TtcProgressBar.Message = $"[{TtcProgressBar.CurrentTick} / {TtcProgressBar.MaxTicks}] \"{filename}\": Processing...";

        /// <summary>
        /// Called when it starts extracting a TTC file with the specified filename.
        /// </summary>
        /// <param name="filename">The filename of the font.</param>
        public void StartExtractingTtc(string filename) => TtcProgressBar.Message = $"[{TtcProgressBar.CurrentTick} / {TtcProgressBar.MaxTicks}] \"{filename}\": Extracting...";

        /// <summary>
        /// Called when it starts processing the extracted files of a TTC file with the specified filename.
        /// </summary>
        /// <param name="filename">The filename of the font.</param>
        public void StartProcessingExtractedFilesOf(string filename) => TtcProgressBar.Message = $"[{TtcProgressBar.CurrentTick} / {TtcProgressBar.MaxTicks}] \"{filename}\": Processing extracted files...";

        /// <summary>
        /// Called when it starts building the TTC file with the specified filename after it's processed.
        /// </summary>
        /// <param name="filename">The filename of the font.</param>
        public void StartBuildingProcessedTtc(string filename) => TtcProgressBar.Message = $"[{TtcProgressBar.CurrentTick} / {TtcProgressBar.MaxTicks}] \"{filename}\": Building...";

        /// <summary>
        /// Called after all TTC files are processed.
        /// </summary>
        public void DoneProcessingTtc()
        {
            TtcProgressBar.Message = $"[{TtcProgressBar.CurrentTick} / {TtcProgressBar.MaxTicks}] Done processing TTC files.";
            OverallProgressBar.Message =
                $"[{OverallProgressBar.CurrentTick} / {OverallProgressBar.MaxTicks}] Done processing TTC files.";
        }

        /// <summary>
        /// Called after the TTC file with the specified filename is done processing.
        /// </summary>
        /// <param name="filename">The filename of the font.</param>
        public void DoneProcessingTtc(string filename)
        {
            TtcProgressBar.Tick($"[{TtcProgressBar.CurrentTick + 1} / {TtcProgressBar.MaxTicks}] \"{filename}\": Done processing.");
            OverallProgressBar.Tick($"[{OverallProgressBar.CurrentTick + 1} / {OverallProgressBar.MaxTicks}] Processing TTC files...");
        }
        #endregion

        public void MarkDone() => OverallProgressBar.Message = "Done.";

        public ProgressBar(int numTtf, int numTtc)
        {
            NumTtf = numTtf;
            NumTtc = numTtc;

            OverallProgressBar = new ShellProgressBar.ProgressBar(numTtf + numTtc,
                $"{numTtf + numTtc} fonts to be processed.", OverallProgressBarOptions);
        }

        public void Dispose()
        {
            OverallProgressBar?.Dispose();
            TtfProgressBar?.Dispose();
            TtcProgressBar?.Dispose();
        }
    }
}
