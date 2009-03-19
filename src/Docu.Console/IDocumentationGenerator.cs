using System;
using System.Collections.Generic;
using System.Reflection;

namespace Docu
{
    public interface IDocumentationGenerator
    {
        void SetAssemblies(IEnumerable<string> assemblyPaths);
        void SetAssemblies(IEnumerable<Assembly> assembliesToParse);
        void SetXmlFiles(IEnumerable<string> xmlFiles);
        void SetXmlContent(IEnumerable<string> xmlContents);
        void SetTemplatePath(string templateDirectory);
        void SetOutputPath(string outputDirectory);
        void Generate();
        event EventHandler<BadFileEventArgs> BadFileEvent;
    }
}