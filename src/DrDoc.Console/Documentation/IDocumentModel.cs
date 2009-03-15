using System.Collections.Generic;
using DrDoc.Parsing.Model;

namespace DrDoc.Documentation
{
    public interface IDocumentModel
    {
        IList<Namespace> Create(IEnumerable<IDocumentationMember> members);
    }
}