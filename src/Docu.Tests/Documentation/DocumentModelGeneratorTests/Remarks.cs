using System.Collections.Generic;
using Docu.Documentation;
using Docu.Documentation.Comments;
using Docu.Parsing.Comments;
using Example;
using NUnit.Framework;
using Rhino.Mocks;

namespace Docu.Tests.Documentation.DocumentModelGeneratorTests
{
    [TestFixture]
    public class Remarks : BaseDocumentModelGeneratorFixture
    {
        [Test]
        public void ShouldHaveRemarksForType()
        {
            var model = new DocumentModel(new CommentParser(), StubEventAggregator);
            var members = new[]
            {
                Type<First>(@"<member name=""T:Example.First""><remarks>First remark</remarks></member>"),
            };
            var namespaces = model.Create(members);
            var comment = new List<IComment>(namespaces[0].Types[0].Remarks.Children);

            comment.Count.ShouldEqual(1);
            ((InlineText)comment[0]).Text.ShouldEqual("First remark");
        }

        [Test]
        public void ShouldPassRemarksToContentParser()
        {
            var contentParser = MockRepository.GenerateMock<ICommentParser>();
            var model = new DocumentModel(contentParser, StubEventAggregator);
            var members = new[] { Type<First>(@"<member name=""T:Example.First""><remarks>First remark</remarks></member>") };

            contentParser.Stub(x => x.Parse(null))
                .IgnoreArguments()
                .Return(new List<IComment>());

            model.Create(members);

            contentParser.AssertWasCalled(x => x.Parse(members[0].Xml.ChildNodes[0]));
        }

        [Test]
        public void ShouldHaveRemarksForMethods()
        {
            var model = new DocumentModel(new CommentParser(), StubEventAggregator);
            var members = new[]
            {
                Method<Second>(@"<member name=""M:Example.Second.SecondMethod2(System.String,System.Int32)""><remarks>Second method 2</remarks></member>", x => x.SecondMethod2(null, 0))
            };
            var namespaces = model.Create(members);
            var comment = new List<IComment>(namespaces[0].Types[0].Methods[0].Remarks.Children);

            comment.Count.ShouldEqual(1);
            ((InlineText)comment[0]).Text.ShouldEqual("Second method 2");
        }

        [Test]
        public void ShouldPassMethodRemarksToContentParser()
        {
            var contentParser = MockRepository.GenerateMock<ICommentParser>();
            var model = new DocumentModel(contentParser, StubEventAggregator);
            var members = new[] { Method<Second>(@"<member name=""M:Example.Second.SecondMethod""><remarks>First remark</remarks></member>", x => x.SecondMethod()) };

            contentParser.Stub(x => x.Parse(null))
                .IgnoreArguments()
                .Return(new List<IComment>());

            model.Create(members);

            contentParser.AssertWasCalled(x => x.Parse(members[0].Xml.ChildNodes[0]));
        }

        [Test]
        public void ShouldHaveRemarksForProperties()
        {
            var model = new DocumentModel(new CommentParser(), StubEventAggregator);
            var members = new[]
            {
                Property<Second>(@"<member name=""P:Example.Second.SecondProperty""><remarks>Second property</remarks></member>", x => x.SecondProperty),
            };
            var namespaces = model.Create(members);
            var comment = new List<IComment>(namespaces[0].Types[0].Properties[0].Remarks.Children);

            comment.Count.ShouldEqual(1);
            ((InlineText)comment[0]).Text.ShouldEqual("Second property");
        }

        [Test]
        public void ShouldHaveRemarksForEvents()
        {
            var model = new DocumentModel(new CommentParser(), StubEventAggregator);
            var members = new[]
            {
                Event<Second>(@"<member name=""E:Example.Second.AnEvent""><remarks>An event</remarks></member>", "AnEvent"),
            };
            var namespaces = model.Create(members);
            var comment = new List<IComment>(namespaces[0].Types[0].Events[0].Remarks.Children);

            comment.Count.ShouldEqual(1);
            ((InlineText)comment[0]).Text.ShouldEqual("An event");
        }

        [Test]
        public void ShouldHaveRemarksForFields()
        {
            var model = new DocumentModel(new CommentParser(), StubEventAggregator);
            var members = new[]
            {
                Field<Second>(@"<member name=""F:Example.Second.aField""><remarks>A field</remarks></member>", x => x.aField),
            };
            var namespaces = model.Create(members);
            var comment = new List<IComment>(namespaces[0].Types[0].Fields[0].Remarks.Children);

            comment.Count.ShouldEqual(1);
            ((InlineText)comment[0]).Text.ShouldEqual("A field");
        }
    }
}