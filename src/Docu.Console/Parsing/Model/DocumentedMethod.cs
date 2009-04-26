using System;
using System.Reflection;
using System.Xml;

namespace Docu.Parsing.Model
{
    public class DocumentedMethod : IDocumentationMember
    {
        public DocumentedMethod(Identifier name, XmlNode xml, MethodInfo method, Type targetType)
        {
            Method = method;
            TargetType = targetType;
            Xml = xml;
            Name = name;
        }

        public Type TargetType { get; set; }
        public MethodInfo Method { get; set; }

        public XmlNode Xml { get; set; }
        public Identifier Name { get; set; }

        public bool Match(Identifier name)
        {
            return Name.Equals(name);
        }
    }
}