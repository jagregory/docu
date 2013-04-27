using System;
using System.Diagnostics;
using System.Xml;

namespace Docu.Parsing.Model
{
    [DebuggerDisplay("Type {Name.Name,nq} for {TargetType.FullName,nq}")]
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
    }
}
