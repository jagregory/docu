using System;

namespace Docu.Events
{
    public interface IEvent
    {
        void PublishedHandler(Action<object> payloadHandler);
        void SubscribedHandler(Action<Action<object>> subscribedHandler);
    }
}