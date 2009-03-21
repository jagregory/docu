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
            new EventAggregator()
                .GetEvent<AnEvent>()
                .Subscribe(e => { });
        }

        [Test]
        public void should_be_able_to_subscribe_via_base_class()
        {
            new EventAggregator()
                .GetEvent<WarningEvent>()
                .Subscribe(x => { });
        }

        [Test]
        public void should_be_able_to_publish()
        {
            new EventAggregator()
                .GetEvent<AnEvent>()
                .Publish("A message");
        }

        [Test]
        public void subscribers_handler_should_get_called_when_published()
        {
            var wasCalled = false;
            var aggregator = new EventAggregator();

            aggregator
                .GetEvent<AnEvent>()
                .Subscribe(x => { wasCalled = true; });
            aggregator
                .GetEvent<AnEvent>()
                .Publish("Whee!");

            wasCalled.ShouldBeTrue();
        }

        [Test]
        public void subscribers_handler_should_get_called_when_baseclass_published()
        {
            var bigWasCalled = false;
            var smallWasCalled = false;
            var aggregator = new EventAggregator();

            aggregator
                .GetEvent<WarningEvent>()
                .Subscribe(x =>
                {
                    if (x == "big")
                        bigWasCalled = true;
                    if (x == "small")
                        smallWasCalled = true;
                });
            aggregator.GetEvent<BigWarningEvent>().Publish("big");
            aggregator.GetEvent<SmallWarningEvent>().Publish("small");

            bigWasCalled.ShouldBeTrue();
            smallWasCalled.ShouldBeTrue();
        }

        private class AnEvent : DocuEvent<string>
        {
        }

        private class BigWarningEvent : WarningEvent
        {
            
        }

        private class SmallWarningEvent : WarningEvent
        {
            
        }

        private class WarningEvent : DocuEvent<string>
        {}
    }
}