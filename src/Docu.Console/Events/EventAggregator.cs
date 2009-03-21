using System;
using System.Collections.Generic;

namespace Docu.Events
{
    public class EventAggregator : IEventAggregator
    {
        private readonly IDictionary<Func<Type, bool>, Action<object>> handlers = new Dictionary<Func<Type, bool>, Action<object>>();

        private void Publish<TEvent>(object payload)
        {
            foreach (var handler in handlers)
            {
                if (handler.Key(typeof(TEvent)))
                    handler.Value(payload);
            }
        }

        private void Subscribe<TEvent>(Action<object> handler)
        {
            handlers.Add(CanHandle<TEvent>, handler);
        }

        private bool CanHandle<TEvent>(Type arg)
        {
            if (arg == typeof(TEvent)) return true;
            if (arg.IsSubclassOf(typeof(TEvent))) return true;
            if (typeof(TEvent).IsAssignableFrom(arg)) return true;

            return false;
        }

        public TEvent GetEvent<TEvent>()
            where TEvent : IEvent, new()
        {
            var ev = new TEvent();

            ev.PublishedHandler(Publish<TEvent>);
            ev.SubscribedHandler(Subscribe<TEvent>);

            return ev;
        }
    }
}