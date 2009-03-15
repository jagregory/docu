using System.Collections.Generic;
using DrDoc.Model;

namespace DrDoc.Parsing
{
    public interface IAssociationTransformer
    {
        IList<DocNamespace> Transform(IEnumerable<IDocumentationMember> members);
    }
}