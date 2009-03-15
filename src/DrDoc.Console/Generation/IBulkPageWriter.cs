using System.Collections.Generic;
using DrDoc.Documentation;

namespace DrDoc.Generation
{
    public interface IBulkPageWriter
    {
        void CreatePagesFromDirectory(string templatePath, string destination, IList<Namespace> namespaces);
    }
}