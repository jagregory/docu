using System.Collections.Generic;
using DrDoc.Documentation;

namespace DrDoc.Generation
{
    public interface IPageWriter
    {
        void CreatePages(string templateDirectory, string destination, IList<Namespace> namespaces);
        void SetTemplatePath(string templateDirectory);
    }
}