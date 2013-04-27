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

        public Identifier Name { get; private set; }
        public Type TargetType { get; private set; }
        public XmlNode Xml { get; private set; }

        public bool Match(Identifier name)
        {
            return Name.Equals(name);
        }
    }
}