using Docu.Documentation;
using Docu.Parsing.Model;
using Example;
using NUnit.Framework;

namespace Docu.Tests.Documentation.DocumentModelGeneratorTests
{
    [TestFixture]
    public class Fields : BaseDocumentModelGeneratorFixture
    {
        [Test]
        public void ShouldHaveEventsInTypes()
        {
            var model = new DocumentModel(StubParser, StubEventAggregator);
            var members = new IDocumentationMember[]
            {
                Type<Second>(@"<member name=""T:Example.Second"" />"),  
                Field<Second>(@"<member name=""F:Example.Second.aField"" />", x => x.aField),
            };
            var namespaces = model.Create(members);
            var field = Field<Second>(x => x.aField);

            namespaces[0].Types[0].Fields
                .ShouldContain(x => x.IsIdentifiedBy(Identifier.FromField(field, typeof(Second))));
        }

        [Test]
        public void ShouldForceTypeIfOnlyEventDefined()
        {
            var model = new DocumentModel(StubParser, StubEventAggregator);
            var members = new[] { Field<Second>(@"<member name=""F:Example.Second.aField"" />", x => x.aField) };
            var namespaces = model.Create(members);

            namespaces[0].Name.ShouldEqual("Example");
            namespaces[0].Types.ShouldContain(x => x.IsIdentifiedBy(Identifier.FromType(typeof(Second))));
        }
    }
}