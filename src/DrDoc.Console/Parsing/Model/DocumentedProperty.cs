using System.Reflection;
using System.Xml;

namespace DrDoc.Parsing.Model
{
    public class DocumentedProperty : IDocumentationMember
    {
        public DocumentedProperty(Identifier name, XmlNode xml, PropertyInfo property)
        {
            Property = property;
            Xml = xml;
            Name = name;
        }

        public PropertyInfo Property { get; set; }
        public XmlNode Xml { get; set; }
        public Identifier Name { get; set; }

        public bool Match(Identifier name)
        {
            return Name == name;
        }
    }
}