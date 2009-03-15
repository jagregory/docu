using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DrDoc.Console;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using TestFixture = NUnit.Framework.TestFixtureAttribute;
using Test = NUnit.Framework.TestAttribute;
using SetUp = NUnit.Framework.SetUpAttribute;

namespace DrDoc.Tests.Console.ConsoleApplicationTests
{
    [TestFixture]
    public class ValidArguments
    {
        private IScreenWriter StubWriter;

        [SetUp]
        public void create_stubs()
        {
            StubWriter = MockRepository.GenerateStub<IScreenWriter>();
        }

        [Test]
        public void should_pass_assemblies_to_docgen()
        {
            var docGen = MockRepository.GenerateMock<IDocumentationGenerator>();
            var app = new ConsoleApplication(StubWriter, docGen);

            app.SetArguments(new[] { "DrDoc.Tests.dll", "DummyDocs.xml" });
            app.Run();

            docGen.AssertWasCalled(x => x.SetAssemblies(new string[0]),
                                   x => x.Constraints(List.ContainsAll(new[] {"DrDoc.Tests.dll"})));
        }

        [Test]
        public void should_pass_xmls_to_docgen()
        {
            var docGen = MockRepository.GenerateMock<IDocumentationGenerator>();
            var app = new ConsoleApplication(StubWriter, docGen);

            app.SetArguments(new[] { "DrDoc.Tests.dll", "DummyDocs.xml" });
            app.Run();

            docGen.AssertWasCalled(x => x.SetXmlFiles(new string[0]),
                                   x => x.Constraints(List.ContainsAll(new[] { "DummyDocs.xml" })));
        }

        [Test]
        public void if_all_good_should_generate()
        {
            var docGen = MockRepository.GenerateMock<IDocumentationGenerator>();
            var app = new ConsoleApplication(StubWriter, docGen);

            app.SetArguments(new[] { "DrDoc.Tests.dll", "DummyDocs.xml" });
            app.Run();

            docGen.AssertWasCalled(x => x.Generate());
        }

        [Test]
        public void if_all_good_should_show_done_after_generate()
        {
            var writer = MockRepository.GenerateMock<IScreenWriter>();
            var docGen = MockRepository.GenerateMock<IDocumentationGenerator>();
            var app = new ConsoleApplication(writer, docGen);

            app.SetArguments(new[] { "DrDoc.Tests.dll", "DummyDocs.xml" });
            app.Run();

            docGen.AssertWasCalled(x => x.Generate());
            writer.AssertWasCalled(x => x.WriteMessage(null),
                x => x.Constraints(Is.TypeOf<DoneMessage>()));
        }
    }
}
