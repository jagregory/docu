using System;
using System.Diagnostics;
using System.Reflection;
using System.Xml;

namespace Docu.Parsing.Model
{
    [DebuggerDisplay("Event {Name.Name,nq} for {TargetType.FullName,nq}")]
    public class DocumentedEvent : IDocumentationMember
    {
        public DocumentedEvent(Identifier name, XmlNode xml, EventInfo ev, Type targetType)
        {
            Name = name;
            Xml = xml;
            Event = ev;
            TargetType = targetType;
        }

        public EventInfo Event { get; private set; }

        public Identifier Name { get; private set; }
        public Type TargetType { get; private set; }
        public XmlNode Xml { get; private set; }
    }
}
