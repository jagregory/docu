using System.Linq;
using Docu.Documentation;
using Docu.Documentation.Comments;
using Docu.Parsing.Comments;
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
            var model = new DocumentModel(new CommentParser(), StubEventAggregator);
            var properties = new[]
            {
                Property<Second>(@"<member name=""P:Example.Second.SecondProperty""><value>The string representation.</value></member>", x => x.SecondProperty)
            };
            var namespaces = model.Create(properties);

            namespaces[0].Types[0].Properties[0].Value.Children.Count().ShouldEqual(1);
            ((InlineText)namespaces[0].Types[0].Properties[0].Value.Children.First()).Text.ShouldEqual("The string representation.");
        }

        [Test]
        public void ShouldHaveValueForMethod()
        {
            var model = new DocumentModel(new CommentParser(), StubEventAggregator);
            var methods = new[]
            {
                Method<ReturnMethodClass>(@"<member name=""Example.ReturnMethodClass""><value>A string.</value></member>", x => x.Method())
            };
            var namespaces = model.Create(methods);

            namespaces[0].Types[0].Methods[0].Value.Children.Count().ShouldEqual(1);
            ((InlineText)namespaces[0].Types[0].Methods[0].Value.Children.First()).Text.ShouldEqual("A string.");
        }
    }
}
