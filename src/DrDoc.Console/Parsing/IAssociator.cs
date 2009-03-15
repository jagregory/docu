using System;
using System.Collections.Generic;
using System.Xml;
using DrDoc.Model;

namespace DrDoc.Parsing
{
    public interface IAssociator
    {
        IList<IDocumentationMember> AssociateMembersWithXml(IList<IDocumentationMember> undocumentedMembers, XmlNode[] snippets);
    }
}