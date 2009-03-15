using System.Collections.Generic;
using DrDoc.Documentation;

namespace DrDoc.Generation
{
    public interface IPatternTemplateResolver
    {
        IList<TemplateMatch> Resolve(string path, IList<Namespace> namespaces);
    }
}