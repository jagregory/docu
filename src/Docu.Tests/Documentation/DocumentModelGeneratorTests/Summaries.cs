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
    public class Summaries : BaseDocumentModelGeneratorFixture
    {
        [Test]
        public void ShouldHaveSummaryForType()
        {
            var model = new DocumentationModelBuilder(RealParser, new EventAggregator());
            var members = new[]
            {
                Type<First>(@"<member name=""T:Example.First""><summary>First summary</summary></member>"),
            };
            var namespaces = model.CombineToTypeHierarchy(members);
            var comment = new List<Comment>(namespaces[0].Types[0].Summary.Children);

            comment.Count.ShouldEqual(1);
            ((InlineText)comment[0]).Text.ShouldEqual("First summary");
        }

        [Test]
        public void ShouldPassSummaryToContentParser()
        {
            var contentParser = MockRepository.GenerateMock<ICommentParser>();
            var model = new DocumentationModelBuilder(contentParser, new EventAggregator());
            var members = new[] { Type<First>(@"<member name=""T:Example.First""><summary>First summary</summary></member>") };

            contentParser.Stub(x => x.ParseNode(null))
                .IgnoreArguments()
                .Return(new List<Comment>());

            model.CombineToTypeHierarchy(members);

            contentParser.AssertWasCalled(x => x.ParseNode(members[0].Xml.ChildNodes[0]));
        }

        [Test]
        public void ShouldHaveSummaryForMethods()
        {
            var model = new DocumentationModelBuilder(RealParser, new EventAggregator());
            var members = new IDocumentationMember[]
            {
                Type<Second>(@"<member name=""T:Example.Second"" />"),
                Method<Second>(@"<member name=""M:Example.Second.SecondMethod2(System.String,System.Int32)""><summary>Second method 2</summary></member>", x => x.SecondMethod2(null, 0))
            };
            var namespaces = model.CombineToTypeHierarchy(members);
            var comment = new List<Comment>(namespaces[0].Types[0].Methods[0].Summary.Children);

            comment.Count.ShouldEqual(1);
            ((InlineText)comment[0]).Text.ShouldEqual("Second method 2");
        }

        [Test]
        public void ShouldPassMethodSummaryToContentParser()
        {
            var contentParser = MockRepository.GenerateMock<ICommentParser>();
            var model = new DocumentationModelBuilder(contentParser, new EventAggregator());
            var members = new[] { Method<Second>(@"<member name=""M:Example.Second.SecondMethod""><summary>First summary</summary></member>", x => x.SecondMethod()) };

            contentParser.Stub(x => x.ParseNode(null))
                .IgnoreArguments()
                .Return(new List<Comment>());

            model.CombineToTypeHierarchy(members);

            contentParser.AssertWasCalled(x => x.ParseNode(members[0].Xml.ChildNodes[0]));
        }

        [Test]
        public void ShouldHaveSummaryForProperties()
        {
            var model = new DocumentationModelBuilder(RealParser, new EventAggregator());
            var members = new IDocumentationMember[]
            {
                Type<Second>(@"<member name=""T:Example.Second"" />"),
                Property<Second>(@"<member name=""P:Example.Second.SecondProperty""><summary>Second property</summary></member>", x => x.SecondProperty),
            };
            var namespaces = model.CombineToTypeHierarchy(members);
            var comment = new List<Comment>(namespaces[0].Types[0].Properties[0].Summary.Children);

            comment.Count.ShouldEqual(1);
            ((InlineText)comment[0]).Text.ShouldEqual("Second property");
        }

        [Test]
        public void ShouldHaveSummaryForEvents()
        {
            var model = new DocumentationModelBuilder(RealParser, new EventAggregator());
            var members = new IDocumentationMember[]
            {
                Type<Second>(@"<member name=""T:Example.Second"" />"),
                Event<Second>(@"<member name=""E:Example.Second.AnEvent""><summary>An event</summary></member>", "AnEvent"),
            };
            var namespaces = model.CombineToTypeHierarchy(members);
            var comment = new List<Comment>(namespaces[0].Types[0].Events[0].Summary.Children);

            comment.Count.ShouldEqual(1);
            ((InlineText)comment[0]).Text.ShouldEqual("An event");
        }

        [Test]
        public void ShouldHaveSummaryForFields()
        {
            var model = new DocumentationModelBuilder(RealParser, new EventAggregator());
            var members = new IDocumentationMember[]
            {
                Type<Second>(@"<member name=""T:Example.Second"" />"),
                Field<Second>(@"<member name=""F:Example.Second.aField""><summary>A field</summary></member>", x => x.aField),
            };
            var namespaces = model.CombineToTypeHierarchy(members);
            var comment = new List<Comment>(namespaces[0].Types[0].Fields[0].Summary.Children);

            comment.Count.ShouldEqual(1);
            ((InlineText)comment[0]).Text.ShouldEqual("A field");
        }

        [Test]
        public void ShouldHaveSummaryForMethodParameter()
        {
            var model = new DocumentationModelBuilder(RealParser, new EventAggregator());
            var members = new IDocumentationMember[]
            {
                Type<Second>(@"<member name=""T:Example.Second"" />"),
                Method<Second>(@"
                <member name=""M:Example.Second.SecondMethod2(System.String,System.Int32)"">
                  <param name=""one"">First parameter</param>
                  <param name=""two"">Second parameter</param>
                </member>", x => x.SecondMethod2(null, 0))
            };
            var namespaces = model.CombineToTypeHierarchy(members);
            var comment1 = new List<Comment>(namespaces[0].Types[0].Methods[0].Parameters[0].Summary.Children);
            var comment2 = new List<Comment>(namespaces[0].Types[0].Methods[0].Parameters[1].Summary.Children);

            comment1.Count.ShouldEqual(1);
            ((InlineText)comment1[0]).Text.ShouldEqual("First parameter");
            comment2.Count.ShouldEqual(1);
            ((InlineText)comment2[0]).Text.ShouldEqual("Second parameter");
        }

        [Test]
        public void ShouldPassMethodParameterSummaryToContentParser()
        {
            var contentParser = MockRepository.GenerateMock<ICommentParser>();
            var model = new DocumentationModelBuilder(contentParser, new EventAggregator());
            var members = new[]
            {
                Method<Second>(@"
                <member name=""M:Example.Second.SecondMethod2(System.String,System.Int32)"">
                  <param name=""one"">First parameter</param>
                  <param name=""two"">Second parameter</param>
                </member>", x => x.SecondMethod2(null, 0))
            };

            contentParser.Stub(x => x.ParseNode(null))
                .IgnoreArguments()
                .Return(new List<Comment>());

            model.CombineToTypeHierarchy(members);

            contentParser.AssertWasCalled(x => x.ParseNode(members[0].Xml.ChildNodes[0]));
            contentParser.AssertWasCalled(x => x.ParseNode(members[0].Xml.ChildNodes[1]));
        }

    }
}