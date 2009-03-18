using System.Collections.Generic;
using System.Reflection;
using Docu.Documentation;

namespace Docu.Parsing
{
    public interface IAssemblyXmlParser
    {
        IList<AssemblyDoc> CreateDocumentModel(IEnumerable<Assembly> assemblies, IEnumerable<string> xml);
    }
}