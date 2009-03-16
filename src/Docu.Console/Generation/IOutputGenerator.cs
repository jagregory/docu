namespace Docu.Generation
{
    public interface IOutputGenerator
    {
        string Convert(string templateName, OutputData data);
    }
}