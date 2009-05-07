using System.IO;

namespace Docu.Generation
{
    public class UntransformableResourceManager : IUntransformableResourceManager
    {
        public void MoveResources(string templatePath, string outputPath)
        {
            foreach (string file in Directory.GetFiles(templatePath, "*", SearchOption.AllDirectories))
            {
                if (Path.GetExtension(file) == ".spark") continue;
                if ( (File.GetAttributes(file) & FileAttributes.Hidden) == FileAttributes.Hidden) continue;
                var containingDirectory = Path.GetDirectoryName(file);
                if ( (File.GetAttributes(containingDirectory) & FileAttributes.Hidden) == FileAttributes.Hidden) continue;

                string newPath = file.Replace(templatePath, outputPath);
                string newDirectory = Path.GetDirectoryName(newPath);

                if (!Directory.Exists(newDirectory))
                    Directory.CreateDirectory(newDirectory);

                File.Copy(file, newPath, true);
            }
        }
    }
}