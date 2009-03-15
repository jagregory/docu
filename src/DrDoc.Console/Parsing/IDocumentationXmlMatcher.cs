using System;
using System.Collections.Generic;
using System.Xml;
using DrDoc.Parsing.Model;

namespace DrDoc.Parsing
{
    public interface IDocumentationXmlMatcher
    {
        IList<IDocumentationMember> DocumentMembers(IList<IDocumentationMember> undocumentedMembers, XmlNode[] snippets);
    }
}