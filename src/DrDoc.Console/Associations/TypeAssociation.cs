using System;
using System.Xml;
using DrDoc.Associations;

namespace DrDoc.Associations
{
    public class TypeAssociation : Association
    {
        public TypeAssociation(MemberName name, XmlNode xml, Type type)
            : base(name, xml)
        {
            Type = type;
        }

        public Type Type { get; set; }
    }
}