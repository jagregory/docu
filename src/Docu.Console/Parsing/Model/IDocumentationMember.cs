using System.Xml;

namespace Docu.Parsing.Model
{
    public interface IDocumentationMember
    {
        XmlNode Xml { get; set; }
        Identifier Name { get; }
        bool Match(Identifier name);
    }
}