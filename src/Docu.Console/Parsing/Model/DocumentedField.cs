using System;
using System.Diagnostics;
using System.Reflection;
using System.Xml;

namespace Docu.Parsing.Model
{
    [DebuggerDisplay("Field {Name.Name,nq} for {TargetType.FullName,nq}")]
    public class DocumentedField : IDocumentationMember
    {
        public DocumentedField(Identifier name, XmlNode xml, FieldInfo field, Type targetType)
        {
            Field = field;
            Xml = xml;
            Name = name;
            TargetType = targetType;
        }

        public FieldInfo Field { get; private set; }

        public Identifier Name { get; private set; }
        public Type TargetType { get; private set; }
        public XmlNode Xml { get; private set; }
    }
}
