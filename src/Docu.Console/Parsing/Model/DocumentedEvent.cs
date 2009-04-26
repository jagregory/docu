using System;
using System.Reflection;
using System.Xml;

namespace Docu.Parsing.Model
{
    public class DocumentedEvent : IDocumentationMember
    {
        public DocumentedEvent(Identifier name, XmlNode xml, EventInfo ev, Type targetType)
        {
            Name = name;
            Xml = xml;
            Event = ev;
            TargetType = targetType;
        }

        public Type TargetType { get; set; }
        public EventInfo Event { get; set; }

        public XmlNode Xml { get; set; }
        public Identifier Name { get; set; }

        public bool Match(Identifier name)
        {
            return Name.Equals(name);
        }
    }
}