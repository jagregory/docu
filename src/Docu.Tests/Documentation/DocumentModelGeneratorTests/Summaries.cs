using System.Collections.Generic;
using Docu.Documentation;
using Docu.Documentation.Comments;
using Docu.Parsing;
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
            var members = new[]
            {
                Type<First>(@"<member name=""T:Example.First""><summary>First summary</summary></member>"),
                Type<Second>(@"<member name=""T:Example.Second""><summary>Second summary</summary></member>"),
            };
            var namespaces = model.Create(members);

            namespaces[0].Types[0].Summary.CountShouldEqual(1);
            ((InlineText)namespaces[0].Types[0].Summary[0]).Text.ShouldEqual("First summary");
            namespaces[0].Types[1].Summary.CountShouldEqual(1);
            ((InlineText)namespaces[0].Types[1].Summary[0]).Text.ShouldEqual("Second summary");
        }

        [Test]
        public void ShouldPassSummaryToContentParser()
        {
            var contentParser = MockRepository.GenerateMock<ICommentContentParser>();
            var members = new[] { Type<First>(@"<member name=""T:Example.First""><summary>First summary</summary></member>") };

            contentParser.Stub(x => x.Parse(null))
                .IgnoreArguments()
                .Return(new List<IComment>());

            new DocumentModel(contentParser).Create(members);

            contentParser.AssertWasCalled(x => x.Parse(members[0].Xml.ChildNodes[0]));
        }

        [Test]
        public void ShouldHaveSummaryForMethods()
        {
            var members = new[]
            {
                Method<Second>(@"<member name=""M:Example.Second.SecondMethod""><summary>Second method</summary></member>", x => x.SecondMethod()),
                Method<Second>(@"<member name=""M:Example.Second.SecondMethod2(System.String,System.Int32)""><summary>Second method 2</summary></member>", x => x.SecondMethod2(null, 0))
            };
            var namespaces = model.Create(members);

            namespaces[0].Types[0].Methods[0].Summary.CountShouldEqual(1);
            ((InlineText)namespaces[0].Types[0].Methods[0].Summary[0]).Text.ShouldEqual("Second method");
            namespaces[0].Types[0].Methods[1].Summary.CountShouldEqual(1);
            ((InlineText)namespaces[0].Types[0].Methods[1].Summary[0]).Text.ShouldEqual("Second method 2");
        }

        [Test]
        public void ShouldPassMethodSummaryToContentParser()
        {
            var contentParser = MockRepository.GenerateMock<ICommentContentParser>();
            var members = new[] { Method<Second>(@"<member name=""M:Example.Second.SecondMethod""><summary>First summary</summary></member>", x => x.SecondMethod()) };

            contentParser.Stub(x => x.Parse(null))
                .IgnoreArguments()
                .Return(new List<IComment>());

            new DocumentModel(contentParser).Create(members);

            contentParser.AssertWasCalled(x => x.Parse(members[0].Xml.ChildNodes[0]));
        }

        [Test]
        public void ShouldHaveSummaryForProperties()
        {
            var members = new[]
            {
                Property<Second>(@"<member name=""P:Example.Second.SecondProperty""><summary>Second property</summary></member>", x => x.SecondProperty),
            };
            var namespaces = model.Create(members);

            namespaces[0].Types[0].Properties[0].Summary.CountShouldEqual(1);
            ((InlineText)namespaces[0].Types[0].Properties[0].Summary[0]).Text.ShouldEqual("Second property");
        }

        [Test]
        public void ShouldHaveSummaryForEvents()
        {
            var members = new[]
            {
                Event<Second>(@"<member name=""E:Example.Second.AnEvent""><summary>An event</summary></member>", "AnEvent"),
            };
            var namespaces = model.Create(members);

            namespaces[0].Types[0].Events[0].Summary.CountShouldEqual(1);
            ((InlineText)namespaces[0].Types[0].Events[0].Summary[0]).Text.ShouldEqual("An event");
        }

        [Test]
        public void ShouldHaveSummaryForMethodParameter()
        {
            var members = new[]
            {
                Method<Second>(@"
                <member name=""M:Example.Second.SecondMethod2(System.String,System.Int32)"">
                  <param name=""one"">First parameter</param>
                  <param name=""two"">Second parameter</param>
                </member>", x => x.SecondMethod2(null, 0))
            };
            var namespaces = model.Create(members);

            namespaces[0].Types[0].Methods[0].Parameters[0].Summary.CountShouldEqual(1);
            ((InlineText)namespaces[0].Types[0].Methods[0].Parameters[0].Summary[0]).Text.ShouldEqual("First parameter");
            namespaces[0].Types[0].Methods[0].Parameters[1].Summary.CountShouldEqual(1);
            ((InlineText)namespaces[0].Types[0].Methods[0].Parameters[1].Summary[0]).Text.ShouldEqual("Second parameter");
        }

        [Test]
        public void ShouldPassMethodParameterSummaryToContentParser()
        {
            var contentParser = MockRepository.GenerateMock<ICommentContentParser>();
            var members = new[]
            {
                Method<Second>(@"
                <member name=""M:Example.Second.SecondMethod2(System.String,System.Int32)"">
                  <param name=""one"">First parameter</param>
                  <param name=""two"">Second parameter</param>
                </member>", x => x.SecondMethod2(null, 0))
            };

            contentParser.Stub(x => x.Parse(null))
                .IgnoreArguments()
                .Return(new List<IComment>());

            new DocumentModel(contentParser).Create(members);

            contentParser.AssertWasCalled(x => x.Parse(members[0].Xml.ChildNodes[0]));
            contentParser.AssertWasCalled(x => x.Parse(members[0].Xml.ChildNodes[1]));
        }
    }
}