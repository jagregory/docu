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

        public Type TargetType { get; set; }
        public PropertyInfo Property { get; set; }

        public XmlNode Xml { get; set; }
        public Identifier Name { get; set; }

        public bool Match(Identifier name)
        {
            return Name.Equals(name);
        }
    }
}