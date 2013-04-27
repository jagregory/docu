namespace Docu.Events
{
    using System;
    using System.Collections.Generic;

    public class EventAggregator
    {
        readonly IDictionary<EventType, Action<string>> handlers = new Dictionary<EventType, Action<string>>();

        public void Publish(EventType eventType, string payload)
        {
            Action<string> handler;
            if (handlers.TryGetValue(eventType, out handler))
            {
                handler(payload);
            }
        }

        public void Subscribe(EventType eventType, Action<string> handle)
        {
            handlers.Add(eventType, handle);
        }
    }
}
