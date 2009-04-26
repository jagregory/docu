using System;
using System.Reflection;
using System.Xml;

namespace Docu.Parsing.Model
{
    public class DocumentedField : IDocumentationMember
    {
        public DocumentedField(Identifier name, XmlNode xml, FieldInfo field, Type targetType)
        {
            Field = field;
            Xml = xml;
            Name = name;
            TargetType = targetType;
        }

        public Type TargetType { get; set; }
        public FieldInfo Field { get; set; }

        public XmlNode Xml { get; set; }
        public Identifier Name { get; set; }

        public bool Match(Identifier name)
        {
            return Name.Equals(name);
        }
    }
}