using System;
using System.Xml;

namespace DrDoc.Associations
{
    public abstract class Association
    {
        public Association(MemberName name, XmlNode xml)
        {
            Name = name;
            Xml = xml;
        }

        public XmlNode Xml { get; set; }
        public MemberName Name { get; private set; }

        public bool Match(MemberName name)
        {
            return Name == name;
        }
    }
}