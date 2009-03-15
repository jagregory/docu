using System.Collections.Generic;
using DrDoc.Documentation;

namespace DrDoc.Generation
{
    public interface ITemplateTransformer
    {
        void Transform(string templatePath, IList<Namespace> namespaces);
    }
}