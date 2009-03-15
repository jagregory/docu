using System.Collections.Generic;
using System.Reflection;
using DrDoc.Generation;
using DrDoc.IO;
using DrDoc.Parsing;

namespace DrDoc
{
    public class DocumentationGenerator : IDocumentationGenerator
    {
        private readonly IAssemblyLoader assemblyLoader;
        private readonly IXmlLoader xmlLoader;
        private readonly IAssemblyXmlParser parser;
        private readonly IBulkPageWriter writer;
        private readonly IUntransformableResourceManager resourceManager;
        private readonly List<Assembly> assemblies = new List<Assembly>();
        private readonly List<string> xmls = new List<string>();
        private string templatePath = "templates";
        private string outputPath = "output";

        public DocumentationGenerator(IAssemblyLoader assemblyLoader, IXmlLoader xmlLoader, IAssemblyXmlParser parser, IBulkPageWriter writer, IUntransformableResourceManager resourceManager)
        {
            this.assemblyLoader = assemblyLoader;
            this.xmlLoader = xmlLoader;
            this.parser = parser;
            this.writer = writer;
            this.resourceManager = resourceManager;
        }

        public void SetAssemblies(IEnumerable<string> assemblyPaths)
        {
            foreach (var assemblyPath in assemblyPaths)
            {
                assemblies.Add(assemblyLoader.LoadFrom(assemblyPath));
            }
        }

        public void SetAssemblies(IEnumerable<Assembly> assembliesToParse)
        {
            assemblies.AddRange(assembliesToParse);
        }

        public void SetXmlFiles(IEnumerable<string> xmlFiles)
        {
            foreach (var xmlFile in xmlFiles)
            {
                xmls.Add(xmlLoader.LoadFrom(xmlFile));
            }
        }

        public void SetXmlContent(IEnumerable<string> xmlContents)
        {
            xmls.AddRange(xmlContents);
        }

        public void SetTemplatePath(string templateDirectory)
        {
            templatePath = templateDirectory;
        }

        public void SetOutputPath(string outputDirectory)
        {
            outputPath = outputDirectory;
        }

        public void Generate()
        {
            var documentModel = parser.CreateDocumentModel(assemblies, xmls);
            
            writer.CreatePagesFromDirectory(templatePath, outputPath, documentModel);

            resourceManager.MoveResources(templatePath, outputPath);
        }
    }
}