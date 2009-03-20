using System.Linq;
using Docu.Console;
using Docu.Parsing;
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
        public void should_show_warning_if_warning_raised()
        {
            var writer = MockRepository.GenerateMock<IScreenWriter>();
            var docGen = MockRepository.GenerateMock<IDocumentationGenerator>();
            var app = new ConsoleApplication(writer, docGen);

            docGen.Raise(x => x.Warning += null, docGen, new GenerationEventArgs("Warning!", WarningType.DocumentModel));

            writer.AssertWasCalled(x => x.WriteMessage(null),
                                   x => x.Constraints(IsMessage<DocumentModelWarningMessage>("WARNING: Warning!")));
                                       
        }

        private AbstractConstraint IsMessage<T>(string message)
        {
            return Is.Matching<IScreenMessage>(c => c is T && c.GetBody().FirstOrDefault(b => b == message) != null);
        }
    }
}