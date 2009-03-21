namespace Docu.Events
{
    public interface IEventAggregator
    {
        TEvent GetEvent<TEvent>()
            where TEvent : IEvent, new();
    }
}