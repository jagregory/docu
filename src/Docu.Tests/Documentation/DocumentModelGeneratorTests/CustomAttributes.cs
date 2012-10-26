using System.Linq;
using Docu.Documentation;
using Docu.Parsing.Model;
using Docu.Tests.Utils;
using Example;
using NUnit.Framework;

namespace Docu.Tests.Documentation.DocumentModelGeneratorTests
{
    [TestFixture]
    public class CustomAttributes : BaseDocumentModelGeneratorFixture
    {
        [Test]
        public void ShouldHaveCustomAttributesOnProperties()
        {
            var model = new DocumentModel(StubParser, StubEventAggregator);
            var members = new IDocumentationMember[]
            {
                Type<Second>(@"<member name=""T:Example.Second"" />"),
                Property<Second>(@"<member name=""P:Example.Second.SecondProperty2"" />", x => x.SecondProperty2)
            };
            var namespaces = model.Create(members);
            var property = namespaces[0].Types[0].Properties[0];

            property.Attributes.ShouldNotBeNull();
            property.Attributes.Count.ShouldEqual(1);
            property.Attributes.FirstOrDefault().ShouldBeOfType<CustomAttribute>();
        }
    }
}
