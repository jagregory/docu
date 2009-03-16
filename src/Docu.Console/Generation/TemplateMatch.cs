namespace Docu.Generation
{
    public class TemplateMatch
    {
        public TemplateMatch(string outputPath, string templatePath, OutputData data)
        {
            OutputPath = outputPath;
            TemplatePath = templatePath;
            Data = data;
        }

        public string OutputPath { get; private set; }
        public string TemplatePath { get; private set; }
        public OutputData Data { get; private set; }
    }
}