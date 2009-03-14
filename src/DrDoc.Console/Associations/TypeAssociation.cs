using System;
using System.Xml;
using DrDoc.Associations;

namespace DrDoc.Associations
{
    public class TypeAssociation : Association
    {
        public TypeAssociation(string name, XmlNode xml, Type type)
            : base(name, xml)
        {
            Type = type;
        }

        public Type Type { get; set; }
    }
}