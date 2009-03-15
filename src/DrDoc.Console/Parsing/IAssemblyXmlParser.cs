using System.Collections.Generic;
using System.Reflection;
using DrDoc.Documentation;

namespace DrDoc.Parsing
{
    public interface IAssemblyXmlParser
    {
        IList<Namespace> CreateDocumentModel(IEnumerable<Assembly> assemblies, IEnumerable<string> xml);
    }
}