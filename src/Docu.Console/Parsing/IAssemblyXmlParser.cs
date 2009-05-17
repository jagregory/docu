using System.Collections.Generic;
using System.Reflection;
using Docu.Documentation;

namespace Docu.Parsing
{
    public interface IAssemblyXmlParser
    {
        IList<Namespace> CreateDocumentModel(IEnumerable<Assembly> assemblies, IEnumerable<string> xmlDocumentContents);
    }
}