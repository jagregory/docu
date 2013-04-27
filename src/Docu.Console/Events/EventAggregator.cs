namespace Docu.Events
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The event aggregator.
    /// </summary>
    public class EventAggregator : IEventAggregator
    {
        private readonly IDictionary<Func<Type, bool>, Action<object>> handlers =
            new Dictionary<Func<Type, bool>, Action<object>>();

        public TEvent GetEvent<TEvent>() where TEvent : IEvent, new()
        {
            var ev = new TEvent();

            ev.PublishedHandler(this.Publish<TEvent>);
            ev.SubscribedHandler(this.Subscribe<TEvent>);

            return ev;
        }

        private bool CanHandle<TEvent>(Type arg)
        {
            if (arg == typeof(TEvent))
            {
                return true;
            }

            if (arg.IsSubclassOf(typeof(TEvent)))
            {
                return true;
            }

            if (typeof(TEvent).IsAssignableFrom(arg))
            {
                return true;
            }

            return false;
        }

        private void Publish<TEvent>(object payload)
        {
            foreach (var handler in this.handlers)
            {
                if (handler.Key(typeof(TEvent)))
                {
                    handler.Value(payload);
                }
            }
        }

        private void Subscribe<TEvent>(Action<object> handler)
        {
            this.handlers.Add(this.CanHandle<TEvent>, handler);
        }
    }
}