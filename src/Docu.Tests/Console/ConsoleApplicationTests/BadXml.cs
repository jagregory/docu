using System.Linq;
using Docu.Console;
using Docu.Events;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using Is=Rhino.Mocks.Constraints.Is;

namespace Docu.Tests.Console.ConsoleApplicationTests
{
    [TestFixture]
    public class BadXml
    {
        [Test]
        public void should_show_warning_if_warning_message_created()
        {
            var writer = MockRepository.GenerateMock<IScreenWriter>();
            var docGen = MockRepository.GenerateStub<IDocumentationGenerator>();
            var eventAggregator = new EventAggregator();
            var app = new ConsoleApplication(writer, docGen, eventAggregator);

            eventAggregator
                .GetEvent<WarningEvent>()
                .Publish("Warning!");

            writer.AssertWasCalled(x => x.WriteMessage(null),
                                   x => x.Constraints(IsMessage<WarningMessage>("WARNING: Warning!")));
                                       
        }

        private AbstractConstraint IsMessage<T>(string message)
        {
            return Is.Matching<IScreenMessage>(c => c is T && c.GetBody().FirstOrDefault(b => b == message) != null);
        }
    }
}