using System.Collections.Generic;
using System.IO;
using DrDoc.IO;

namespace DrDoc.Generation
{
    public class TemplateTransformer : ITemplateTransformer
    {
        private const string A_Bang = "!";
        private const string Namespace = "namespace.spark";
        private const string Type = "type.spark";

        private readonly IOutputGenerator generator;
        private readonly IOutputWriter writer;

        public TemplateTransformer(IOutputGenerator generator, IOutputWriter writer)
        {
            this.generator = generator;
            this.writer = writer;
        }

        public void Transform(string templatePath, IList<DocNamespace> namespaces)
        {
            var transformedOutput = new List<OutputFile>();
            var templateName = Path.GetFileName(templatePath);

            if (templateName.StartsWith(A_Bang))
                transformedOutput.AddRange(TransformSpecial(templatePath, namespaces));
            else
            {
                var output = generator.Convert(templatePath, new OutputData { Namespaces = namespaces });
                var filename = templatePath.Replace("spark", "htm");

                transformedOutput.Add(new OutputFile(filename, output));
            }

            foreach (var pair in transformedOutput)
            {
                writer.WriteFile(pair.FileName, pair.Content);
            }
        }

        private IEnumerable<OutputFile> TransformSpecial(string templatePath, IList<DocNamespace> namespaces)
        {
            var specialName = GetSpecialName(templatePath);

            if (specialName == Namespace)
                return TransformNamespaces(templatePath, namespaces);
            if (specialName == Type)
                return TransformTypes(templatePath, namespaces);


            return new OutputFile[0];
        }

        private IEnumerable<OutputFile> TransformNamespaces(string templatePath, IList<DocNamespace> namespaces)
        {
            var transformedOutputs = new List<OutputFile>();

            foreach (var ns in namespaces)
            {
                var output = generator.Convert(templatePath, new OutputData { Namespaces = namespaces, Namespace = ns });
                var filename = templatePath.Replace("!namespace", ns.Name).Replace("spark", "htm");

                transformedOutputs.Add(new OutputFile(filename, output));
            }

            return transformedOutputs;
        }

        private IEnumerable<OutputFile> TransformTypes(string templatePath, IList<DocNamespace> namespaces)
        {
            var transformedOutputs = new List<OutputFile>();

            foreach (var ns in namespaces)
            {
                foreach (var type in ns.Types)
                {
                    var output = generator.Convert(templatePath, new OutputData { Namespaces = namespaces, Namespace = ns, Type = type });
                    var filename = templatePath.Replace("!type", ns.Name + "." + type.Name).Replace("spark", "htm");

                    transformedOutputs.Add(new OutputFile(filename, output));
                }
            }

            return transformedOutputs;
        }

        private string GetSpecialName(string templatePath)
        {
            return Path.GetFileName(templatePath).Substring(1);
        }

        private class OutputFile
        {
            public OutputFile(string filename, string content)
            {
                FileName = filename;
                Content = content;
            }

            public string FileName { get; private set; }
            public string Content { get; private set; }
        }
    }
}