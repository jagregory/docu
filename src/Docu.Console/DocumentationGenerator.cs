using Docu.Events;
using Docu.IO;
using Docu.Output;
using Docu.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Docu.Parsing.Comments;

namespace Docu
{
    public class DocumentationGenerator
    {
        readonly string _outputPath;
        readonly string _templatePath;

        readonly ICommentParser _commentParser;
        readonly EventAggregator _eventAggregator;
        readonly List<Assembly> _assemblies = new List<Assembly>();
        readonly List<string> _contentsOfXmlFiles = new List<string>();

        public DocumentationGenerator(string outputPath, string templatePath, ICommentParser commentParser, EventAggregator eventAggregator)
        {
            _outputPath = outputPath;
            _templatePath = templatePath;
            _commentParser = commentParser;
            _eventAggregator = eventAggregator;
        }

        public void SetAssemblies(IEnumerable<string> assemblyPaths)
        {
            foreach (string assemblyPath in assemblyPaths)
            {
                try
                {
                    _assemblies.Add(Assembly.LoadFrom(assemblyPath));
                }
                catch (BadImageFormatException)
                {
                    _eventAggregator.Publish(EventType.BadFile, assemblyPath);
                }
            }
        }

        public void SetXmlFiles(IEnumerable<string> xmlFiles)
        {
            foreach (string xmlFile in xmlFiles)
            {
                _contentsOfXmlFiles.Add(File.ReadAllText(xmlFile));
            }
        }

        public void Generate()
        {
            if (_assemblies.Count <= 0) return;

            var parser = new DocumentationModelBuilder(_commentParser, _eventAggregator);
            var namespaces = parser.CreateDocumentModel(_assemblies, _contentsOfXmlFiles);

            var writer = new BulkPageWriter(new PageWriter(new HtmlGenerator(), new FileSystemOutputWriter(), new PatternTemplateResolver()));
            writer.SetAssemblies(_assemblies);
            writer.CreatePagesFromDirectory(_templatePath, _outputPath, namespaces);

            var resourceManager = new UntransformableResourceManager();
            resourceManager.MoveResources(_templatePath, _outputPath);
        }
    }
}
