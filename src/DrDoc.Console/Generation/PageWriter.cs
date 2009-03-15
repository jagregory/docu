using System.Collections.Generic;
using System.IO;
using DrDoc.Documentation;
using DrDoc.IO;

namespace DrDoc.Generation
{
    public class PageWriter : IPageWriter
    {
        private readonly IOutputGenerator generator;
        private readonly IOutputWriter writer;
        private readonly IPatternTemplateResolver patternTemplateResolver;

        public PageWriter(IOutputGenerator generator, IOutputWriter writer, IPatternTemplateResolver patternTemplateResolver)
        {
            this.generator = generator;
            this.patternTemplateResolver = patternTemplateResolver;
            this.writer = writer;
        }

        public void CreatePages(string templatePath, string destination, IList<Namespace> namespaces)
        {
            var paths = patternTemplateResolver.Resolve(templatePath, namespaces);

            foreach (var path in paths)
            {
                var output = generator.Convert(path.TemplatePath, path.Data);
                var outputDir = Path.GetDirectoryName(path.OutputPath);
                var outputPath = path.OutputPath;

                if (!string.IsNullOrEmpty(destination))
                {
                    var outputFilename = Path.GetFileName(path.OutputPath);

                    outputDir = Path.Combine(destination, outputDir);
                    outputPath = Path.Combine(outputDir, outputFilename);
                }

                if (outputDir != "" && !writer.Exists(outputDir))
                    writer.CreateDirectory(outputDir);

                    writer.WriteFile(outputPath, output);
            }
        }
    }
}