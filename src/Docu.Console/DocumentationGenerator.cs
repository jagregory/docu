using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Docu.Events;
using Docu.Generation;
using Docu.IO;
using Docu.Parsing;

namespace Docu
{
    public class DocumentationGenerator : IDocumentationGenerator
    {
        private readonly List<Assembly> assemblies = new List<Assembly>();
        private readonly IAssemblyLoader assemblyLoader;
        private readonly IAssemblyXmlParser parser;
        private readonly IUntransformableResourceManager resourceManager;
        private readonly IEventAggregator eventAggregator;
        private readonly IBulkPageWriter writer;
        private readonly IXmlLoader xmlLoader;
        private readonly List<string> xmls = new List<string>();
        private string outputPath = "output";
        private string templatePath = Path.Combine(Path.GetDirectoryName(typeof(DocumentationGenerator).Assembly.Location), "templates");

        public DocumentationGenerator(IAssemblyLoader assemblyLoader, IXmlLoader xmlLoader, IAssemblyXmlParser parser, IBulkPageWriter writer, IUntransformableResourceManager resourceManager, IEventAggregator eventAggregator)
        {
            this.assemblyLoader = assemblyLoader;
            this.xmlLoader = xmlLoader;
            this.parser = parser;
            this.writer = writer;
            this.resourceManager = resourceManager;
            this.eventAggregator = eventAggregator;
        }

        public void SetAssemblies(IEnumerable<string> assemblyPaths)
        {
            foreach (string assemblyPath in assemblyPaths)
            {
                try
                {
                    assemblies.Add(assemblyLoader.LoadFrom(assemblyPath));
                }
                catch(BadImageFormatException)
                {
                    RaiseBadFileEvent(assemblyPath);
                }
            }
        }

        private void RaiseBadFileEvent(string path)
        {
            eventAggregator
                .GetEvent<BadFileEvent>()
                .Publish(path);
        }

        public void SetAssemblies(IEnumerable<Assembly> assembliesToParse)
        {
            assemblies.AddRange(assembliesToParse);
        }

        public void SetXmlFiles(IEnumerable<string> xmlFiles)
        {
            foreach (string xmlFile in xmlFiles)
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

            writer.SetAssemblies(assemblies);
            
            if (assemblies.Count <= 0) return;
            
            writer.CreatePagesFromDirectory(templatePath, outputPath, documentModel);
            resourceManager.MoveResources(templatePath, outputPath);
        }
    }
}