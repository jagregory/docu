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
        readonly List<Assembly> assemblies = new List<Assembly>();
        readonly IAssemblyLoader assemblyLoader;
        readonly AssemblyXmlParser parser;
        readonly IUntransformableResourceManager resourceManager;
        readonly IEventAggregator eventAggregator;
        readonly IBulkPageWriter writer;
        readonly IXmlLoader xmlLoader;
        readonly List<string> contentsOfXmlFiles = new List<string>();
        string outputPath = "output";
        string templatePath = Path.Combine(Path.GetDirectoryName(typeof (DocumentationGenerator).Assembly.Location), "templates");

        public DocumentationGenerator(IAssemblyLoader assemblyLoader, IXmlLoader xmlLoader, AssemblyXmlParser parser, IBulkPageWriter writer, IUntransformableResourceManager resourceManager, IEventAggregator eventAggregator)
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
                catch (BadImageFormatException)
                {
                    RaiseBadFileEvent(assemblyPath);
                }
            }
        }

        void RaiseBadFileEvent(string path)
        {
            eventAggregator
                .GetEvent<BadFileEvent>()
                .Publish(path);
        }

        public void SetXmlFiles(IEnumerable<string> xmlFiles)
        {
            foreach (string xmlFile in xmlFiles)
            {
                contentsOfXmlFiles.Add(xmlLoader.LoadFrom(xmlFile));
            }
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
            var documentModel = parser.CreateDocumentModel(assemblies, contentsOfXmlFiles);

            writer.SetAssemblies(assemblies);

            if (assemblies.Count <= 0) return;

            writer.CreatePagesFromDirectory(templatePath, outputPath, documentModel);
            resourceManager.MoveResources(templatePath, outputPath);
        }
    }
}