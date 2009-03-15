using System.IO;

namespace DrDoc.Generation
{
    public class UntransformableResourceManager : IUntransformableResourceManager
    {
        public void MoveResources(string templatePath, string outputPath)
        {
            foreach (var file in Directory.GetFiles(templatePath, "*", SearchOption.AllDirectories))
            {
                if (Path.GetExtension(file) == ".spark") continue;

                var newPath = file.Replace(templatePath, outputPath);
                var newDirectory = Path.GetDirectoryName(newPath);

                if (!Directory.Exists(newDirectory))
                    Directory.CreateDirectory(newDirectory);

                File.Copy(file, newPath);
            }
        }
    }
}