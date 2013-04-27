using Docu.Documentation;
using Docu.Events;
using Docu.IO;
using Docu.Output;
using Docu.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Docu
{
    public class DocumentationGenerator
    {
        readonly string outputPath;
        readonly string templatePath;
        readonly DocumentModel documentModel;
        readonly IEventAggregator eventAggregator;
        readonly List<Assembly> assemblies = new List<Assembly>();
        readonly List<string> contentsOfXmlFiles = new List<string>();

        public DocumentationGenerator(string outputPath, string templatePath, DocumentModel documentModel, IEventAggregator eventAggregator)
        {
            this.outputPath = outputPath;
            this.templatePath = templatePath;
            this.documentModel = documentModel;
            this.eventAggregator = eventAggregator;
        }

        public void SetAssemblies(IEnumerable<string> assemblyPaths)
        {
            foreach (string assemblyPath in assemblyPaths)
            {
                try
                {
                    assemblies.Add(Assembly.LoadFrom(assemblyPath));
                }
                catch (BadImageFormatException)
                {
                    eventAggregator
                        .GetEvent<BadFileEvent>()
                        .Publish(assemblyPath);
                }
            }
        }

        public void SetXmlFiles(IEnumerable<string> xmlFiles)
        {
            foreach (string xmlFile in xmlFiles)
            {
                contentsOfXmlFiles.Add(File.ReadAllText(xmlFile));
            }
        }

        public void Generate()
        {
            if (assemblies.Count <= 0) return;

            var parser = new AssemblyXmlParser(documentModel);
            var namespaces = parser.CreateDocumentModel(assemblies, contentsOfXmlFiles);

            var writer = new BulkPageWriter(new PageWriter(new HtmlGenerator(), new FileSystemOutputWriter(), new PatternTemplateResolver()));
            writer.SetAssemblies(assemblies);
            writer.CreatePagesFromDirectory(templatePath, outputPath, namespaces);

            var resourceManager = new UntransformableResourceManager();
            resourceManager.MoveResources(templatePath, outputPath);
        }
    }
}
