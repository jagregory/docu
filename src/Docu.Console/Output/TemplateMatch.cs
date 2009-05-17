namespace Docu.Output
{
    public class TemplateMatch
    {
        public TemplateMatch(string outputPath, string templatePath, ViewData data)
        {
            OutputPath = outputPath;
            TemplatePath = templatePath;
            Data = data;
        }

        public string OutputPath { get; private set; }
        public string TemplatePath { get; private set; }
        public ViewData Data { get; private set; }
    }
}