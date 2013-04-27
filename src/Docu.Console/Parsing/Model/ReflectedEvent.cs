using System;
using System.Reflection;

namespace Docu.Parsing.Model
{
    public class ReflectedEvent : DocumentedEvent
    {
        public ReflectedEvent(EventIdentifier name, EventInfo ev, Type targetType)
            : base(name, null, ev, targetType)
        {
            DeclaringName = ev.DeclaringType != targetType
                ? IdentifierFor.Event(ev, ev.DeclaringType)
                : name;
        }

        public Identifier DeclaringName { get; private set; }

        public bool Match(Identifier name)
        {
            return Name.Equals(name);
        }
    }
}
