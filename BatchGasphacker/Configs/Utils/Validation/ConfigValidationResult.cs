using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace BatchGasphacker.Configs.Utils.Validation
{
    public class ConfigValidationResult
    {
        #region InputDirectory
        public DirectoryStatus InputDirectoryStatus { get; set; }

        public IEnumerable<FileInfo> TtfFiles { get; set; }

        public IEnumerable<FileInfo> TtcFiles { get; set; }
        #endregion

        #region OutputDirectory
        public DirectoryStatus OutputDirectoryStatus { get; set; }
        #endregion

        #region TtxExeDirectory
        public DirectoryStatus TtxExeDirectoryStatus { get; set; }
        #endregion

        #region AfdkoDirectory
        public DirectoryStatus AfdkoDirectoryStatus { get; set; }
        #endregion

        public List<string> ReportProblems()
        {
            var problems = new List<string>();

            var temp = InputDirectoryStatus.ToReport(nameof(InputDirectoryStatus));
            if (temp != null) problems.Add(temp);
            temp = OutputDirectoryStatus.ToReport(nameof(OutputDirectoryStatus));
            if (temp != null) problems.Add(temp);
            temp = TtxExeDirectoryStatus.ToReport(nameof(TtxExeDirectoryStatus));
            if (temp != null) problems.Add(temp);
            temp = AfdkoDirectoryStatus.ToReport(nameof(AfdkoDirectoryStatus));
            if (temp != null) problems.Add(temp);

            return problems;
        }
    }

    public abstract class DirectoryStatus
    {
        public static Ok Ok() => new Ok();

        public static NotSpecified NotSpecified() => new NotSpecified();

        public static Invalid Invalid() => new Invalid();

        public static Empty Empty() => new Empty();

        public static FileNotFound FileNotFound(params string[] filenames) => new FileNotFound(filenames);

        public abstract string ToReport(string propertyName);
    }

    public class Ok : DirectoryStatus
    {
        internal Ok() { }

        public override string ToReport(string propertyName) => null;
    }

    public class NotSpecified : DirectoryStatus
    {
        internal NotSpecified() { }

        public override string ToReport(string propertyName) => $"\"{propertyName}\" is not specified.";
    }

    public class Invalid : DirectoryStatus
    {
        internal Invalid() { }

        public override string ToReport(string propertyName) => $"The value of \"{propertyName}\" is not a valid directory.";
    }

    public class Empty : DirectoryStatus
    {
        internal Empty() { }

        public override string ToReport(string propertyName) => $"Directory of \"{propertyName}\" is empty.";
    }

    public class FileNotFound : DirectoryStatus
    {
        public string[] Filenames { get; set; }

        internal FileNotFound(string[] filenames) => Filenames = filenames;

        public override string ToReport(string propertyName)
        {
            var result = $"Required file(s) not found in directory of \"{propertyName}\": ";

            for (var i = 0; i < Filenames.Length; i++)
            {
                result += $"\"{Filenames[i]}\"";
                if (i < Filenames.Length - 1)
                    result += ", ";
            }

            result += ".";
            return result;
        }
    }
}
