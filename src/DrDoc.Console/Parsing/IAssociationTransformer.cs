using System.Collections.Generic;
using DrDoc.Associations;

namespace DrDoc.Parsing
{
    public interface IAssociationTransformer
    {
        IList<DocNamespace> Transform(IEnumerable<Association> associations);
    }
}