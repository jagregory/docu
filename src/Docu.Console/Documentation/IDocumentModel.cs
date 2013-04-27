using System.Collections.Generic;
using Docu.Parsing.Model;

namespace Docu.Documentation
{
    public interface IDocumentModel
    {
        IList<Namespace> Create(IEnumerable<IDocumentationMember> members);
    }
}
