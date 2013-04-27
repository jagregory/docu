using System.Collections.Generic;
using Docu.Documentation;
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
    public class Remarks : BaseDocumentModelGeneratorFixture
    {
        [Test]
        public void ShouldHaveRemarksForType()
        {
            var model = new DocumentationModelBuilder(RealParser, new EventAggregator());
            var members = new[]
            {
                Type<First>(@"<member name=""T:Example.First""><remarks>First remark</remarks></member>"),
            };
            var namespaces = model.CombineToTypeHierarchy(members);
            var comment = new List<Comment>(namespaces[0].Types[0].Remarks.Children);

            comment.Count.ShouldEqual(1);
            ((InlineText)comment[0]).Text.ShouldEqual("First remark");
        }

        [Test]
        public void ShouldPassRemarksToContentParser()
        {
            var contentParser = MockRepository.GenerateMock<ICommentParser>();
            var model = new DocumentationModelBuilder(contentParser, new EventAggregator());
            var members = new[] { Type<First>(@"<member name=""T:Example.First""><remarks>First remark</remarks></member>") };

            contentParser.Stub(x => x.ParseNode(null))
                .IgnoreArguments()
                .Return(new List<Comment>());

            model.CombineToTypeHierarchy(members);

            contentParser.AssertWasCalled(x => x.ParseNode(members[0].Xml.ChildNodes[0]));
        }

        [Test]
        public void ShouldHaveRemarksForMethods()
        {
            var model = new DocumentationModelBuilder(RealParser, new EventAggregator());
            var members = new IDocumentationMember[]
            {
                Type<Second>(@"<member name=""T:Example.Second"" />"),
                Method<Second>(@"<member name=""M:Example.Second.SecondMethod2(System.String,System.Int32)""><remarks>Second method 2</remarks></member>", x => x.SecondMethod2(null, 0))
            };
            var namespaces = model.CombineToTypeHierarchy(members);
            var comment = new List<Comment>(namespaces[0].Types[0].Methods[0].Remarks.Children);

            comment.Count.ShouldEqual(1);
            ((InlineText)comment[0]).Text.ShouldEqual("Second method 2");
        }

        [Test]
        public void ShouldPassMethodRemarksToContentParser()
        {
            var contentParser = MockRepository.GenerateMock<ICommentParser>();
            var model = new DocumentationModelBuilder(contentParser, new EventAggregator());
            var members = new[] { Method<Second>(@"<member name=""M:Example.Second.SecondMethod""><remarks>First remark</remarks></member>", x => x.SecondMethod()) };

            contentParser.Stub(x => x.ParseNode(null))
                .IgnoreArguments()
                .Return(new List<Comment>());

            model.CombineToTypeHierarchy(members);

            contentParser.AssertWasCalled(x => x.ParseNode(members[0].Xml.ChildNodes[0]));
        }

        [Test]
        public void ShouldHaveRemarksForProperties()
        {
            var model = new DocumentationModelBuilder(RealParser, new EventAggregator());
            var members = new IDocumentationMember[]
            {
                Type<Second>(@"<member name=""T:Example.Second"" />"),
                Property<Second>(@"<member name=""P:Example.Second.SecondProperty""><remarks>Second property</remarks></member>", x => x.SecondProperty),
            };
            var namespaces = model.CombineToTypeHierarchy(members);
            var comment = new List<Comment>(namespaces[0].Types[0].Properties[0].Remarks.Children);

            comment.Count.ShouldEqual(1);
            ((InlineText)comment[0]).Text.ShouldEqual("Second property");
        }

        [Test]
        public void ShouldHaveRemarksForEvents()
        {
            var model = new DocumentationModelBuilder(RealParser, new EventAggregator());
            var members = new IDocumentationMember[]
            {
                Type<Second>(@"<member name=""T:Example.Second"" />"),
                Event<Second>(@"<member name=""E:Example.Second.AnEvent""><remarks>An event</remarks></member>", "AnEvent"),
            };
            var namespaces = model.CombineToTypeHierarchy(members);
            var comment = new List<Comment>(namespaces[0].Types[0].Events[0].Remarks.Children);

            comment.Count.ShouldEqual(1);
            ((InlineText)comment[0]).Text.ShouldEqual("An event");
        }

        [Test]
        public void ShouldHaveRemarksForFields()
        {
            var model = new DocumentationModelBuilder(RealParser, new EventAggregator());
            var members = new IDocumentationMember[]
            {
                Type<Second>(@"<member name=""T:Example.Second"" />"),
                Field<Second>(@"<member name=""F:Example.Second.aField""><remarks>A field</remarks></member>", x => x.aField),
            };
            var namespaces = model.CombineToTypeHierarchy(members);
            var comment = new List<Comment>(namespaces[0].Types[0].Fields[0].Remarks.Children);

            comment.Count.ShouldEqual(1);
            ((InlineText)comment[0]).Text.ShouldEqual("A field");
        }
    }
}