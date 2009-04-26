using System;
using System.Xml;

namespace Docu.Parsing.Model
{
    public class DocumentedType : IDocumentationMember
    {
        public DocumentedType(Identifier name, XmlNode xml, Type type)
        {
            TargetType = type;
            Xml = xml;
            Name = name;
        }

        public XmlNode Xml { get; set; }
        public Identifier Name { get; set; }
        public Type TargetType { get; set; }

        public bool Match(Identifier name)
        {
            return Name.Equals(name);
        }
    }
}