using System;
using System.Diagnostics;
using System.Reflection;
using System.Xml;

namespace Docu.Parsing.Model
{
    [DebuggerDisplay("Property {Name.Name,nq} for {TargetType.FullName,nq}")]
    public class DocumentedProperty : IDocumentationMember
    {
        public DocumentedProperty(Identifier name, XmlNode xml, PropertyInfo property, Type targetType)
        {
            Property = property;
            Xml = xml;
            Name = name;
            TargetType = targetType;
        }

        public PropertyInfo Property { get; private set; }

        public Identifier Name { get; private set; }
        public Type TargetType { get; private set; }
        public XmlNode Xml { get; private set; }
    }
}
