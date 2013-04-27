using System;
using System.Diagnostics;
using System.Reflection;
using System.Xml;

namespace Docu.Parsing.Model
{
    [DebuggerDisplay("Method {Name.Name,nq} for {TargetType.FullName,nq}")]
    public class DocumentedMethod : IDocumentationMember
    {
        public DocumentedMethod(Identifier name, XmlNode xml, MethodBase method, Type targetType)
        {
            Method = method;
            TargetType = targetType;
            Xml = xml;
            Name = name;
        }

        public MethodBase Method { get; private set; }

        public Identifier Name { get; private set; }
        public Type TargetType { get; private set; }
        public XmlNode Xml { get; private set; }
    }
}
