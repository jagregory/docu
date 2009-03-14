using System.Collections.Generic;

namespace DrDoc.Generation
{
    public interface IPatternTemplateResolver
    {
        IList<TemplateMatch> Resolve(string path, IList<DocNamespace> namespaces);
    }
}