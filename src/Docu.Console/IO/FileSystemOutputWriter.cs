using System.IO;

namespace Docu.IO
{
    public class FileSystemOutputWriter : IOutputWriter
    {
        public void WriteFile(string fileName, string content)
        {
            File.WriteAllText(fileName, content);
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public bool Exists(string directory)
        {
            return Directory.Exists(directory);
        }
    }
}