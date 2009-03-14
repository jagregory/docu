namespace DrDoc.IO
{
    public interface IOutputWriter
    {
        void WriteFile(string fileName, string content);
        void CreateDirectory(string path);
    }
}