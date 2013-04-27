using Docu.Documentation;
using Docu.Events;
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
            var model = new DocumentModel(StubParser, new EventAggregator());
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
    }
}