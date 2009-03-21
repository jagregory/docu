using System;

namespace Docu.Events
{
    public class DocuEvent<TPayload> : IEvent
    {
        private Action<object> publishedCallback;
        private Action<Action<object>> subscribedCallback;

        void IEvent.PublishedHandler(Action<object> payloadHandler)
        {
            publishedCallback = payloadHandler;
        }

        void IEvent.SubscribedHandler(Action<Action<object>> subscribedHandler)
        {
            subscribedCallback = subscribedHandler;
        }

        public virtual void Publish(TPayload payload)
        {
            if (publishedCallback != null)
                publishedCallback(payload);
        }

        public void Subscribe(Action<TPayload> payload)
        {
            if (subscribedCallback != null)
                subscribedCallback(x => payload((TPayload)x));
        }
    }
}