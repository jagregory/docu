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
    public class Arguments
    {
        private IDocumentationGenerator StubDocGen;

        [SetUp]
        public void create_stubs()
        {
            StubDocGen = MockRepository.GenerateStub<IDocumentationGenerator>();
        }

        [Test]
        public void should_show_help_when_no_args()
        {
            var writer = MockRepository.GenerateMock<IScreenWriter>();

            new ConsoleApplication(writer, StubDocGen).Run();

            writer.AssertWasCalled(x => x.WriteMessage(null),
                                   x => x.Constraints(Is.TypeOf<HelpMessage>()));
        }

        [Test]
        public void should_exit_after_showing_help_when_no_args()
        {
            var writer = MockRepository.GenerateMock<IScreenWriter>();

            new ConsoleApplication(writer, StubDocGen).Run();

            writer.AssertWasCalled(x => x.WriteMessage(null),
                                   x => x.Constraints(Is.TypeOf<HelpMessage>()));
            writer.AssertWasNotCalled(x => x.WriteMessage(null),
                                   x => x.Constraints(Is.TypeOf<SplashMessage>()));
        }

        [Test]
        public void shouldnt_show_help_when_has_args()
        {
            var writer = MockRepository.GenerateMock<IScreenWriter>();
            var app = new ConsoleApplication(writer, StubDocGen);
            
            app.SetArguments(new[] { "one" });
            app.Run();

            writer.AssertWasNotCalled(x => x.WriteMessage(null),
                                   x => x.Constraints(Is.TypeOf<HelpMessage>()));
        }

        [Test]
        public void should_show_splash_when_has_args()
        {
            var writer = MockRepository.GenerateMock<IScreenWriter>();
            var app = new ConsoleApplication(writer, StubDocGen);

            app.SetArguments(new[] { "one" });
            app.Run();

            writer.AssertWasCalled(x => x.WriteMessage(null),
                                   x => x.Constraints(Is.TypeOf<SplashMessage>()));
        }

        [Test]
        public void should_show_invalid_message_when_has_bad_arg()
        {
            var writer = MockRepository.GenerateMock<IScreenWriter>();
            var app = new ConsoleApplication(writer, StubDocGen);

            app.SetArguments(new[] { "one" });
            app.Run();

            writer.AssertWasCalled(x => x.WriteMessage(null),
                                   x => x.Constraints(Is.TypeOf<InvalidArgumentMessage>()));
        }

        [Test]
        public void should_show_missing_assembly_message_when_has_bad_assembly_arg()
        {
            var writer = MockRepository.GenerateMock<IScreenWriter>();
            var app = new ConsoleApplication(writer, StubDocGen);

            app.SetArguments(new[] { "missing-file.dll" });
            app.Run();

            writer.AssertWasCalled(x => x.WriteMessage(null),
                                   x => x.Constraints(Is.TypeOf<AssemblyNotFoundMessage>()));
        }

        [Test]
        public void should_show_missing_xml_message_when_has_bad_xml_arg()
        {
            var writer = MockRepository.GenerateMock<IScreenWriter>();
            var app = new ConsoleApplication(writer, StubDocGen);

            app.SetArguments(new[] { "DrDoc.Tests.dll", "missing-file.xml" });
            app.Run();

            writer.AssertWasCalled(x => x.WriteMessage(null),
                                   x => x.Constraints(Is.TypeOf<XmlNotFoundMessage>()));
        }
    }
}
