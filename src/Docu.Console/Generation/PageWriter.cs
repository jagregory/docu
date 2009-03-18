using System.Collections.Generic;
using System.IO;
using Docu.Documentation;
using Docu.IO;

namespace Docu.Generation
{
    public class PageWriter : IPageWriter
    {
        private readonly IOutputGenerator generator;
        private readonly IPatternTemplateResolver patternTemplateResolver;
        private readonly IOutputWriter writer;
        private string templatePath;

        public PageWriter(IOutputGenerator generator, IOutputWriter writer,
                          IPatternTemplateResolver patternTemplateResolver)
        {
            this.generator = generator;
            this.patternTemplateResolver = patternTemplateResolver;
            this.writer = writer;
        }

        public void CreatePages(string templateDirectory, string destination, IList<AssemblyDoc> assemblies)
        {
            IList<TemplateMatch> paths = patternTemplateResolver.Resolve(templateDirectory, assemblies);

            foreach (TemplateMatch path in paths)
            {
                string output = generator.Convert(path.TemplatePath, path.Data);
                string outputDir = Path.GetDirectoryName(path.OutputPath);
                string outputPath = path.OutputPath;

                if (!string.IsNullOrEmpty(destination))
                {
                    string outputFilename = Path.GetFileName(path.OutputPath);

                    if (!string.IsNullOrEmpty(templatePath))
                        outputDir = outputDir.Replace(templatePath, destination);
                    else
                        outputDir = Path.Combine(destination, outputDir);

                    outputPath = Path.Combine(outputDir, outputFilename);
                }

                if (outputDir != "" && !writer.Exists(outputDir))
                    writer.CreateDirectory(outputDir);

                writer.WriteFile(outputPath, output);
            }
        }

        public void SetTemplatePath(string templateDirectory)
        {
            templatePath = templateDirectory;
        }
    }
}