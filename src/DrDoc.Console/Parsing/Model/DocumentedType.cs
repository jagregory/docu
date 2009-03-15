using System;
using System.Xml;

namespace DrDoc.Parsing.Model
{
    public class DocumentedType : IDocumentationMember
    {
        public DocumentedType(Identifier name, XmlNode xml, Type type)
        {
            Type = type;
            Xml = xml;
            Name = name;
        }

        public Type Type { get; set; }
        public XmlNode Xml { get; set; }
        public Identifier Name { get; set; }

        public bool Match(Identifier name)
        {
            return Name == name;
        }
    }
}