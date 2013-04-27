using Docu.Events;
using NUnit.Framework;

namespace Docu.Tests.Events
{
    [TestFixture]
    public class EventAggregatorTests
    {
        [Test]
        public void should_be_able_to_subscribe()
        {
            new EventAggregator().Subscribe(EventType.Warning, e => { });
        }

        [Test]
        public void should_be_able_to_publish()
        {
            new EventAggregator().Publish(EventType.Warning, "A message");
        }

        [Test]
        public void subscribers_handler_should_get_called_when_published()
        {
            var wasCalled1 = false;
            var wasCalled2 = false;

            var aggregator = new EventAggregator();
            aggregator.Subscribe(EventType.Warning, x => { wasCalled1 = true; });
            aggregator.Subscribe(EventType.BadFile, x => { wasCalled2 = true; });
            aggregator.Publish(EventType.Warning, "Whee!");

            wasCalled1.ShouldBeTrue();
            wasCalled2.ShouldBeFalse();
        }
    }
}
