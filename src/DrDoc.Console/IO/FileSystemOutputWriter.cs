using System.IO;

namespace DrDoc.IO
{
    public class FileSystemOutputWriter : IOutputWriter
    {
        public void WriteFile(string fileName, string content)
        {
            File.WriteAllText(fileName, content);
        }
    }
}