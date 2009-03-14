using System;
using System.Reflection;
using System.Xml;
using DrDoc.Associations;

namespace DrDoc.Associations
{
    public class MethodAssociation : Association
    {
        public MethodAssociation(MemberName name, XmlNode xml, MethodInfo method, Type targetType)
            : base(name, xml)
        {
            Method = method;
            TargetType = targetType;
        }

        public Type TargetType { get; set; }
        public MethodInfo Method { get; set; }
    }
}