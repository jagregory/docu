using Docu.Documentation;
using Docu.Parsing.Model;
using Example;
using Example.Deep;
using NUnit.Framework;

namespace Docu.Tests.Documentation.DocumentModelGeneratorTests
{
    [TestFixture]
    public class Namespaces : BaseDocumentModelGeneratorFixture
    {
        [Test]
        public void ShouldBuildNamespaces()
        {
            var model = new DocumentModel(StubParser, StubEventAggregator);
            var members = new[]
            {
                Type<First>(@"<member name=""T:Example.First"" />"),  
                Type<DeepFirst>(@"<member name=""T:Example.Deep.DeepFirst"" />"),
            };
            var namespaces = model.Create(members);

            namespaces.ShouldContain(x => x.IsIdentifiedBy(Identifier.FromNamespace("Example")));
            namespaces.ShouldContain(x => x.IsIdentifiedBy(Identifier.FromNamespace("Example.Deep")));
        }
    }
}