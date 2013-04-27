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

        public Identifier Name { get; private set; }
        public Type TargetType { get; private set; }
        public XmlNode Xml { get; private set; }
        public FieldInfo Field { get; private set; }

        public bool Match(Identifier name)
        {
            return Name.Equals(name);
        }
    }
}