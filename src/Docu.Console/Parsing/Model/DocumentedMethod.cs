using System;
using System.Reflection;
using System.Xml;

namespace Docu.Parsing.Model
{
    public class DocumentedMethod : IDocumentationMember
    {
        public DocumentedMethod(Identifier name, XmlNode xml, MethodBase method, Type targetType)
        {
            Method = method;
            TargetType = targetType;
            Xml = xml;
            Name = name;
        }

        public Identifier Name { get; private set; }
        public Type TargetType { get; private set; }
        public XmlNode Xml { get; private set; }
        public MethodBase Method { get; private set; }

        public bool Match(Identifier name)
        {
            return Name.Equals(name);
        }
    }
}