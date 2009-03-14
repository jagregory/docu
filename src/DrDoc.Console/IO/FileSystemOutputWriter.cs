using System.IO;

namespace DrDoc.IO
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
    }
}