using System.Collections.Generic;
using System.Xml;
using Docu.Parsing.Model;

namespace Docu.Parsing
{
    public interface IDocumentationXmlMatcher
    {
        IList<IDocumentationMember> DocumentMembers(IEnumerable<IDocumentationMember> undocumentedMembers, IEnumerable<XmlNode> snippets);
    }
}