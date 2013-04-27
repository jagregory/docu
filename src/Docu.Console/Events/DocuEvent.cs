namespace Docu.Events
{
    using System;

    public class DocuEvent<TPayload> : IEvent
    {
        private Action<object> publishedCallback;

        private Action<Action<object>> subscribedCallback;

        public virtual void Publish(TPayload payload)
        {
            if (this.publishedCallback != null)
            {
                this.publishedCallback(payload);
            }
        }

        public void Subscribe(Action<TPayload> payload)
        {
            if (this.subscribedCallback != null)
            {
                this.subscribedCallback(x => payload((TPayload)x));
            }
        }

        void IEvent.PublishedHandler(Action<object> payloadHandler)
        {
            this.publishedCallback = payloadHandler;
        }

        void IEvent.SubscribedHandler(Action<Action<object>> subscribedHandler)
        {
            this.subscribedCallback = subscribedHandler;
        }
    }
}