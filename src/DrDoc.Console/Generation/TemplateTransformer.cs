using System.Collections.Generic;
using System.IO;
using DrDoc.IO;

namespace DrDoc.Generation
{
    public class TemplateTransformer : ITemplateTransformer
    {
        private readonly IOutputGenerator generator;
        private readonly IOutputWriter writer;
        private readonly IPatternTemplateResolver patternTemplateResolver;

        public TemplateTransformer(IOutputGenerator generator, IOutputWriter writer, IPatternTemplateResolver patternTemplateResolver)
        {
            this.generator = generator;
            this.patternTemplateResolver = patternTemplateResolver;
            this.writer = writer;
        }

        public void Transform(string templatePath, IList<DocNamespace> namespaces)
        {
            var paths = patternTemplateResolver.Resolve(templatePath, namespaces);

            foreach (var path in paths)
            {
                var output = generator.Convert(path.TemplatePath, path.Data);
                var outputDir = Path.GetDirectoryName(path.OutputPath);

                if (outputDir != "" && !writer.Exists(outputDir))
                    writer.CreateDirectory(outputDir);

                writer.WriteFile(path.OutputPath, output);
            }
        }
    }
}