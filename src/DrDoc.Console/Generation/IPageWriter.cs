using System.Collections.Generic;
using DrDoc.Documentation;

namespace DrDoc.Generation
{
    public interface IPageWriter
    {
        void CreatePages(string templatePath, string destination, IList<Namespace> namespaces);
    }
}