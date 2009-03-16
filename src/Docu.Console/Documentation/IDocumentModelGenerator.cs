using System.Collections.Generic;
using Docu.Parsing.Model;

namespace Docu.Documentation
{
    public interface IDocumentModelGenerator
    {
        IList<Namespace> Create(IEnumerable<IDocumentationMember> members);
    }
}