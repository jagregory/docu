namespace Docu.Output
{
    public interface IOutputGenerator
    {
        string Convert(string templateName, ViewData data);
        void SetTemplatePath(string path);
    }
}