using System;
using System.Reflection;

namespace Docu.Parsing.Model
{
    public class UndocumentedEvent : DocumentedEvent
    {
        public UndocumentedEvent(EventIdentifier name, EventInfo ev, Type targetType)
            : base(name, null, ev, targetType)
        {}
    }
}