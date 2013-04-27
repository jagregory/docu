using Docu.Events;
using Docu.Parsing;
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
            var model = new DocumentationModelBuilder(StubParser, new EventAggregator());
            var members = new[]
            {
                Type<First>(@"<member name=""T:Example.First"" />"),  
                Type<DeepFirst>(@"<member name=""T:Example.Deep.DeepFirst"" />"),
            };
            var namespaces = model.CombineToTypeHierarchy(members);

            namespaces.ShouldContain(x => x.IsIdentifiedBy(IdentifierFor.Namespace("Example")));
            namespaces.ShouldContain(x => x.IsIdentifiedBy(IdentifierFor.Namespace("Example.Deep")));
        }
    }
}