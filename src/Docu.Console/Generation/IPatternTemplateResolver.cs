using System.Collections.Generic;
using Docu.Documentation;

namespace Docu.Generation
{
    public interface IPatternTemplateResolver
    {
        IList<TemplateMatch> Resolve(string path, IList<Namespace> namespaces);
    }
}