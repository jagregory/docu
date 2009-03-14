using System;
using System.Xml;

namespace DrDoc.Associations
{
    public abstract class Association
    {
        public Association(string name, XmlNode xml)
        {
            Name = name;
            Xml = xml;
        }

        public XmlNode Xml { get; set; }
        public string Name { get; private set; }

        public bool Match(string name)
        {
            return Name == name;
        }
    }
}