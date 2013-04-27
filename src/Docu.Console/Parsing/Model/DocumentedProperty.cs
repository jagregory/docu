using System;
using System.Reflection;
using System.Xml;

namespace Docu.Parsing.Model
{
    public class DocumentedProperty : IDocumentationMember
    {
        public DocumentedProperty(Identifier name, XmlNode xml, PropertyInfo property, Type targetType)
        {
            Property = property;
            Xml = xml;
            Name = name;
            TargetType = targetType;
        }

        public Identifier Name { get; private set; }
        public Type TargetType { get; private set; }
        public XmlNode Xml { get; private set; }
        public PropertyInfo Property { get; private set; }

        public bool Match(Identifier name)
        {
            return Name.Equals(name);
        }
    }
}