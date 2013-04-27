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

        public Identifier Name { get; private set; }
        public Type TargetType { get; private set; }
        public XmlNode Xml { get; private set; }
        public EventInfo Event { get; private set; }

        public bool Match(Identifier name)
        {
            return Name.Equals(name);
        }
    }
}