using System.Collections.Generic;
using System.Linq;
using Docu.Documentation;
using Docu.Documentation.Comments;
using Docu.Parsing.Comments;
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
            var model = new DocumentModel(new CommentParser(), StubEventAggregator);
            var members = new[]
            {
                Method<Second>(@"<member name=""M:Example.Second.ReturnType""><returns>Method with return</returns></member>", x => x.ReturnType()),
            };
            var namespaces = model.Create(members);

            namespaces[0].Types[0].Methods[0].Returns.Children.Count().ShouldEqual(1);
            ((InlineText)namespaces[0].Types[0].Methods[0].Returns.Children.First()).Text.ShouldEqual("Method with return");
        }

        [Test]
        public void ShouldPassMethodReturnsToContentParser()
        {
            var contentParser = MockRepository.GenerateMock<ICommentParser>();
            var model = new DocumentModel(contentParser, StubEventAggregator);
            var members = new[] { Method<Second>(@"<member name=""M:Example.Second.ReturnType""><returns>Method with return</returns></member>", x => x.ReturnType()), };

            contentParser.Stub(x => x.Parse(null))
                .IgnoreArguments()
                .Return(new List<IComment>());

            model.Create(members);

            contentParser.AssertWasCalled(x => x.Parse(members[0].Xml.ChildNodes[0]));
        }
    }
}