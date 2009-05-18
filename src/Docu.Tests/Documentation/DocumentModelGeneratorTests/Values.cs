using System.Linq;
using Docu.Documentation;
using Docu.Documentation.Comments;
using Docu.Parsing.Comments;
using Docu.Parsing.Model;
using Example;
using NUnit.Framework;

namespace Docu.Tests.Documentation.DocumentModelGeneratorTests
{
    [TestFixture]
    public class Values : BaseDocumentModelGeneratorFixture
    {
        [Test]
        public void ShouldHaveValueForProperty()
        {
            var model = new DocumentModel(RealParser, StubEventAggregator);
            var properties = new IDocumentationMember[]
            {
                Type<Second>(@"<member name=""T:Example.Second"" />"),
                Property<Second>(@"<member name=""P:Example.Second.SecondProperty""><value>The string representation.</value></member>", x => x.SecondProperty)
            };
            var namespaces = model.Create(properties);

            namespaces[0].Types[0].Properties[0].Value.Children.Count().ShouldEqual(1);
            ((InlineText)namespaces[0].Types[0].Properties[0].Value.Children.First()).Text.ShouldEqual("The string representation.");
        }

        [Test]
        public void ShouldHaveValueForMethod()
        {
            var model = new DocumentModel(RealParser, StubEventAggregator);
            var methods = new IDocumentationMember[]
            {
                Type<ReturnMethodClass>(@"<member name=""T:Example.ReturnMethodClass"" />"),
                Method<ReturnMethodClass>(@"<member name=""Example.ReturnMethodClass""><value>A string.</value></member>", x => x.Method())
            };
            var namespaces = model.Create(methods);

            namespaces[0].Types[0].Methods[0].Value.Children.Count().ShouldEqual(1);
            ((InlineText)namespaces[0].Types[0].Methods[0].Value.Children.First()).Text.ShouldEqual("A string.");
        }
    }
}
