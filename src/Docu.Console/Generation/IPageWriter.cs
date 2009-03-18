using System.Collections.Generic;
using Docu.Documentation;

namespace Docu.Generation
{
    public interface IPageWriter
    {
        void CreatePages(string templateDirectory, string destination, IList<AssemblyDoc> assemblies);
        void SetTemplatePath(string templateDirectory);
    }
}