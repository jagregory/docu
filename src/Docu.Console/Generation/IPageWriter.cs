using System.Collections.Generic;
using Docu.Documentation;

namespace Docu.Generation
{
    public interface IPageWriter
    {
        void CreatePages(string templateDirectory, string destination, IList<Namespace> namespaces);
        void SetTemplatePath(string templateDirectory);
    }
}