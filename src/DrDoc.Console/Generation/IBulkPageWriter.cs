using System.Collections.Generic;
using DrDoc.Documentation;

namespace DrDoc.Generation
{
    public interface IBulkPageWriter
    {
        void CreatePagesFromDirectory(string path, string destination, IList<Namespace> namespaces);
    }
}