using System.Collections.Generic;
using System.Reflection;
using Docu.Documentation;

namespace Docu.Generation
{
    public interface IPatternTemplateResolver
    {
        IList<TemplateMatch> Resolve(string path, IList<Namespace> namespaces);
        void SetAssemblies(IEnumerable<Assembly> assemblies);
    }
}