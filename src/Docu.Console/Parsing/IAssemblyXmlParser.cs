using System;
using System.Collections.Generic;
using System.Reflection;
using Docu.Documentation;

namespace Docu.Parsing
{
    public interface IAssemblyXmlParser
    {
        event EventHandler<ParserWarningEventArgs> ParseWarning;
        IList<Namespace> CreateDocumentModel(IEnumerable<Assembly> assemblies, IEnumerable<string> xml);
    }
}