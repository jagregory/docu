using Docu.Console;
using Docu.Events;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using TestFixture = NUnit.Framework.TestFixtureAttribute;
using Test = NUnit.Framework.TestAttribute;
using SetUp = NUnit.Framework.SetUpAttribute;

namespace Docu.Tests.Console.ConsoleApplicationTests
{
    [TestFixture]
    public class Switches
    {
        private IScreenWriter StubWriter;
        private IDocumentationGenerator StubDocGen;
        private IEventAggregator StubEventAggregator;

        [SetUp]
        public void create_stubs()
        {
            StubWriter = MockRepository.GenerateStub<IScreenWriter>();
            StubDocGen = MockRepository.GenerateStub<IDocumentationGenerator>();
            StubEventAggregator = MockRepository.GenerateStub<IEventAggregator>();
            StubEventAggregator.Stub(x => x.GetEvent<WarningEvent>()).Return(new WarningEvent());
            StubEventAggregator.Stub(x => x.GetEvent<BadFileEvent>()).Return(new BadFileEvent());
        }

        [Test]
        public void should_show_help_if_help_switch_given()
        {
            var writer = MockRepository.GenerateMock<IScreenWriter>();
            var app = new ConsoleApplication(writer, StubDocGen, StubEventAggregator);

            app.SetArguments(new[] { "--help" });
            app.Run();

            writer.AssertWasCalled(x => x.WriteMessage(null),
                x => x.Constraints(Is.TypeOf<HelpMessage>()));
        }

        [Test]
        public void should_show_help_if_help_switch_given_regardless_of_other_args()
        {
            var writer = MockRepository.GenerateMock<IScreenWriter>();
            var app = new ConsoleApplication(writer, StubDocGen, StubEventAggregator);

            app.SetArguments(new[] { "Assembly.dll", "--help", "Assembly.xml" });
            app.Run();

            writer.AssertWasCalled(x => x.WriteMessage(null),
                x => x.Constraints(Is.TypeOf<HelpMessage>()));
        }
    }
}