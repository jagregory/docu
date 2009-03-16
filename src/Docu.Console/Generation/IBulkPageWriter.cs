using System.Collections.Generic;
using Docu.Documentation;

namespace Docu.Generation
{
    public interface IBulkPageWriter
    {
        void CreatePagesFromDirectory(string templatePath, string destination, IList<Namespace> namespaces);
    }
}