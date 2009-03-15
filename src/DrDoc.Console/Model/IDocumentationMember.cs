using System.Xml;

namespace DrDoc.Model
{
    public interface IDocumentationMember
    {
        XmlNode Xml { get; set; }
        Identifier Name { get; }
        bool Match(Identifier name);
    }
}