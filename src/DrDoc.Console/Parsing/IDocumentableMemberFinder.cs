using System;
using System.Collections.Generic;
using DrDoc.Model;

namespace DrDoc.Parsing
{
    public interface IDocumentableMemberFinder
    {
        IList<IDocumentationMember> GetMembersForDocumenting(IEnumerable<Type> types);
    }
}