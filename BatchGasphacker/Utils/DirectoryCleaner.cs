using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchGasphacker.Utils
{
    public static class DirectoryCleaner
    {
        public static async Task CleanDirectoryAsync(string path)
        {
            await Task.Run(() =>
            {
                var files = Directory.GetFiles(path);
                var isDirectoryCleaned = !files.Any();
                if (isDirectoryCleaned)
                    return;

                var watcher = new FileSystemWatcher(path);
                watcher.Deleted += (sender, args) =>
                {
                    // List the directory and mark it cleaned only if there's no file in it
                    if (!Directory.EnumerateFiles(path).Any())
                        isDirectoryCleaned = true;
                };
                watcher.EnableRaisingEvents = true;
                foreach (var tempFile in files)
                    File.Delete(tempFile);

                while (!isDirectoryCleaned)
                { }

                watcher.EnableRaisingEvents = false;
            });
        }
    }
}
