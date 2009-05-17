using System.Collections.Generic;
using System.Reflection;
using Docu.Documentation;

namespace Docu.Generation
{
    public interface IPageWriter
    {
        void CreatePages(string templateDirectory, string destination, IList<Namespace> namespaces);
        void SetTemplatePath(string templateDirectory);
        void SetAssemblies(IEnumerable<Assembly> assemblies);
    }
}