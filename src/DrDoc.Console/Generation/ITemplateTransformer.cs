using System.Collections.Generic;

namespace DrDoc.Generation
{
    public interface ITemplateTransformer
    {
        void Transform(string templatePath, IList<DocNamespace> namespaces);
    }
}