using System;
using System.Collections.Generic;
using Docu.Parsing.Model;

namespace Docu.Parsing
{
    public interface IDocumentableMemberFinder
    {
        IEnumerable<IDocumentationMember> GetMembersForDocumenting(IEnumerable<Type> types);
    }
}