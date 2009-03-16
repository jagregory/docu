using System;
using System.Collections.Generic;
using Docu.Parsing.Model;

namespace Docu.Parsing
{
    public interface IDocumentableMemberFinder
    {
        IList<IDocumentationMember> GetMembersForDocumenting(IEnumerable<Type> types);
    }
}