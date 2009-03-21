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
    public class ValidArguments
    {
        private IScreenWriter StubWriter;
        private IEventAggregator StubEventAggregator;

        [SetUp]
        public void create_stubs()
        {
            StubWriter = MockRepository.GenerateStub<IScreenWriter>();
            StubEventAggregator = MockRepository.GenerateStub<IEventAggregator>();
            StubEventAggregator.Stub(x => x.GetEvent<WarningEvent>()).Return(new WarningEvent());
            StubEventAggregator.Stub(x => x.GetEvent<BadFileEvent>()).Return(new BadFileEvent());
        }

        [Test]
        public void should_pass_assemblies_to_docgen()
        {
            var docGen = MockRepository.GenerateMock<IDocumentationGenerator>();
            var app = new ConsoleApplication(StubWriter, docGen, StubEventAggregator);

            app.SetArguments(new[] { "Docu.Tests.dll", "DummyDocs.xml" });
            app.Run();

            docGen.AssertWasCalled(x => x.SetAssemblies(new string[0]),
                                   x => x.Constraints(List.ContainsAll(new[] { "Docu.Tests.dll" })));
        }

        [Test]
        public void should_pass_xmls_to_docgen()
        {
            var docGen = MockRepository.GenerateMock<IDocumentationGenerator>();
            var app = new ConsoleApplication(StubWriter, docGen, StubEventAggregator);

            app.SetArguments(new[] { "Docu.Tests.dll", "DummyDocs.xml" });
            app.Run();

            docGen.AssertWasCalled(x => x.SetXmlFiles(new string[0]),
                                   x => x.Constraints(List.ContainsAll(new[] { "DummyDocs.xml" })));
        }

        [Test]
        public void if_all_good_should_generate()
        {
            var docGen = MockRepository.GenerateMock<IDocumentationGenerator>();
            var app = new ConsoleApplication(StubWriter, docGen, StubEventAggregator);

            app.SetArguments(new[] { "Docu.Tests.dll", "DummyDocs.xml" });
            app.Run();

            docGen.AssertWasCalled(x => x.Generate());
        }

        [Test]
        public void if_all_good_should_show_done_after_generate()
        {
            var writer = MockRepository.GenerateMock<IScreenWriter>();
            var docGen = MockRepository.GenerateMock<IDocumentationGenerator>();
            var app = new ConsoleApplication(writer, docGen, StubEventAggregator);

            app.SetArguments(new[] { "Docu.Tests.dll", "DummyDocs.xml" });
            app.Run();

            docGen.AssertWasCalled(x => x.Generate());
            writer.AssertWasCalled(x => x.WriteMessage(null),
                x => x.Constraints(Is.TypeOf<DoneMessage>()));
        }
    }
}
