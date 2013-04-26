using System;
using System.Reflection;

namespace Docu.Parsing.Model
{
    public class ReflectedEvent : DocumentedEvent
    {
        public ReflectedEvent(EventIdentifier name, EventInfo ev, Type targetType)
            : base(name, null, ev, targetType)
        {}
    }
}