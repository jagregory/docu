using System.Collections.Generic;
using System.IO;

namespace Docu.Output
{
    public class UntransformableResourceManager
    {
        public void MoveResources(string templatePath, string outputPath)
        {
            foreach (var file in AllFilesUnder(templatePath))
            {
                if (Path.GetExtension(file) == ".spark") continue;
                if (IsHiddenPath(file)) continue;

                var newPath = file.Replace(templatePath, outputPath);
                var newDirectory = Path.GetDirectoryName(newPath);

                if (!Directory.Exists(newDirectory))
                {
                    Directory.CreateDirectory(newDirectory);
                }

                File.Copy(file, newPath, true);
            }
        }

        public static IEnumerable<string> AllFilesUnder(string targetDirectory)
        {
            var fileEntries = Directory.GetFiles(targetDirectory);
            foreach (var fileName in fileEntries)
                yield return fileName;

            foreach (var subdirectory in Directory.GetDirectories(targetDirectory))
            {
                if (IsHiddenPath(subdirectory)) continue;
                foreach (var fileName in AllFilesUnder(subdirectory)) yield return fileName;
            }
        }

        private static bool IsHiddenPath(string path)
        {
            return ((File.GetAttributes(path) & FileAttributes.Hidden) == FileAttributes.Hidden);
        }
    }
}