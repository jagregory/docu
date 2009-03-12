using System;
using System.Xml;
using DrDoc.Associations;

namespace DrDoc.Associations
{
    public class TypeAssociation : Association
    {
        public TypeAssociation(XmlNode xml, Type type)
            : base(xml)
        {
            Type = type;
        }

        public Type Type { get; set; }
    }
}