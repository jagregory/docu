using System.Collections.Generic;
using Docu.Parsing.Model;

namespace Docu.Documentation
{
    public interface IDocumentModel
    {
        IList<AssemblyDoc> Create(IEnumerable<IDocumentationMember> members);
    }
}