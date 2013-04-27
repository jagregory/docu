using System.Collections.Generic;
using System.Linq;
using Docu.Documentation.Comments;
using Docu.Events;
using Docu.Parsing;
using Docu.Parsing.Comments;
using Docu.Parsing.Model;
using Example;
using NUnit.Framework;
using Rhino.Mocks;

namespace Docu.Tests.Documentation.DocumentModelGeneratorTests
{
    [TestFixture]
    public class Returns : BaseDocumentModelGeneratorFixture
    {
        [Test]
        public void ShouldHaveReturnsForMethods()
        {
            var model = new DocumentationModelBuilder(RealParser, new EventAggregator());
            var members = new IDocumentationMember[]
            {
                Type<Second>(@"<member name=""T:Example.Second"" />"),
                Method<Second>(@"<member name=""M:Example.Second.ReturnType""><returns>Method with return</returns></member>", x => x.ReturnType()),
            };
            var namespaces = model.CombineToTypeHierarchy(members);

            namespaces[0].Types[0].Methods[0].Returns.Children.Count().ShouldEqual(1);
            ((InlineText)namespaces[0].Types[0].Methods[0].Returns.Children.First()).Text.ShouldEqual("Method with return");
        }

        [Test]
        public void ShouldPassMethodReturnsToContentParser()
        {
            var contentParser = MockRepository.GenerateMock<ICommentParser>();
            var model = new DocumentationModelBuilder(contentParser, new EventAggregator());
            var members = new[] { Method<Second>(@"<member name=""M:Example.Second.ReturnType""><returns>Method with return</returns></member>", x => x.ReturnType()), };

            contentParser.Stub(x => x.ParseNode(null))
                .IgnoreArguments()
                .Return(new List<Comment>());

            model.CombineToTypeHierarchy(members);

            contentParser.AssertWasCalled(x => x.ParseNode(members[0].Xml.ChildNodes[0]));
        }
    }
}