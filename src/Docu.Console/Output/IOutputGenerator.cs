namespace Docu.Output
{
    public interface IOutputGenerator
    {
        string Convert(string templateName, ViewData data, string relativeOutputPath);
        void SetTemplatePath(string path);
    }
}