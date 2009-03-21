using Docu.Console;
using Docu.Documentation;
using Docu.Events;
using Docu.Parsing;
using Docu.Parsing.Model;
using Example;
using NUnit.Framework;
using Rhino.Mocks;

namespace Docu.Tests.Documentation.DocumentModelGeneratorTests
{
    [TestFixture]
    public class UnrecognisedDocumentationTypes : BaseDocumentModelGeneratorFixture
    {
        [Test]
        public void ShouldRaiseWarningOnUnexpectedKindInReferenceInType()
        {
            var ev = MockRepository.GenerateMock<WarningEvent>();
            var model = new DocumentModel(new CommentContentParser(), StubEventAggregator);
            var members = new IDocumentationMember[] { Type<Second>(@"<member name=""T:Example.Second""><summary><see cref=""G:Whats-a-g"" /></summary></member>") };

            StubEventAggregator.Stub(x => x.GetEvent<WarningEvent>())
                .Return(ev);

            model.Create(members);

            ev.AssertWasCalled(x => x.Publish("Unsupported documentation member found: 'G:Whats-a-g'"));
        }

        [Test]
        public void ShouldRaiseWarningOnUnexpectedKindInReferenceInMethod()
        {
            var ev = MockRepository.GenerateMock<WarningEvent>();
            var model = new DocumentModel(new CommentContentParser(), StubEventAggregator);
            var members = new IDocumentationMember[] { Method<Second>(@"<member name=""M:Example.Second.SecondMethod""><summary><see cref=""G:Whats-a-g"" /></summary></member>", x => x.SecondMethod()) };

            StubEventAggregator.Stub(x => x.GetEvent<WarningEvent>())
                .Return(ev);

            model.Create(members);

            ev.AssertWasCalled(x => x.Publish("Unsupported documentation member found: 'G:Whats-a-g'"));
        }

        [Test]
        public void ShouldRaiseWarningOnUnexpectedKindInReferenceInProperty()
        {
            var ev = MockRepository.GenerateMock<WarningEvent>();
            var model = new DocumentModel(new CommentContentParser(), StubEventAggregator);
            var members = new IDocumentationMember[] { Property<Second>(@"<member name=""P:Example.Second.SecondProperty""><summary><see cref=""G:Whats-a-g"" /></summary></member>", x => x.SecondProperty) };

            StubEventAggregator.Stub(x => x.GetEvent<WarningEvent>())
                .Return(ev);

            model.Create(members);

            ev.AssertWasCalled(x => x.Publish("Unsupported documentation member found: 'G:Whats-a-g'"));
        }

        [Test]
        public void ShouldRaiseWarningOnUnexpectedKindInReferenceInEvent()
        {
            var ev = MockRepository.GenerateMock<WarningEvent>();
            var model = new DocumentModel(new CommentContentParser(), StubEventAggregator);
            var members = new IDocumentationMember[] { Event<Second>(@"<member name=""E:Example.Second.AnEvent""><summary><see cref=""G:Whats-a-g"" /></summary></member>", "AnEvent") };

            StubEventAggregator.Stub(x => x.GetEvent<WarningEvent>())
                .Return(ev);

            model.Create(members);

            ev.AssertWasCalled(x => x.Publish("Unsupported documentation member found: 'G:Whats-a-g'"));
        }
    }
}