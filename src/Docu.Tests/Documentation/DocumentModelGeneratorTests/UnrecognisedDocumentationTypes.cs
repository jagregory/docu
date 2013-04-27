using Docu.Events;
using Docu.Parsing;
using Docu.Parsing.Model;
using Example;
using NUnit.Framework;

namespace Docu.Tests.Documentation.DocumentModelGeneratorTests
{
    [TestFixture]
    public class UnrecognisedDocumentationTypes : BaseDocumentModelGeneratorFixture
    {
        [Test]
        public void ShouldRaiseWarningOnUnexpectedKindInReferenceInEvent()
        {
            var events = new EventAggregator();
            var model = new DocumentationModelBuilder(RealParser, events);
            var members = new IDocumentationMember[] {Event<Second>(@"<member name=""E:Example.Second.AnEvent""><summary><see cref=""G:Whats-a-g"" /></summary></member>", "AnEvent")};

            string text = string.Empty;
            events.Subscribe(EventType.Warning, x => text = x);

            model.CombineToTypeHierarchy(members);

            Assert.AreEqual("Unsupported documentation member found: 'G:Whats-a-g'", text);
        }

        [Test]
        public void ShouldRaiseWarningOnUnexpectedKindInReferenceInMethod()
        {
            var events = new EventAggregator();
            var model = new DocumentationModelBuilder(RealParser, events);
            var members = new IDocumentationMember[] {Method<Second>(@"<member name=""M:Example.Second.SecondMethod""><summary><see cref=""G:Whats-a-g"" /></summary></member>", x => x.SecondMethod())};

            string text = string.Empty;
            events.Subscribe(EventType.Warning, x => text = x);

            model.CombineToTypeHierarchy(members);

            Assert.AreEqual("Unsupported documentation member found: 'G:Whats-a-g'", text);
        }

        [Test]
        public void ShouldRaiseWarningOnUnexpectedKindInReferenceInProperty()
        {
            var events = new EventAggregator();
            var model = new DocumentationModelBuilder(RealParser, events);
            var members = new IDocumentationMember[] {Property<Second>(@"<member name=""P:Example.Second.SecondProperty""><summary><see cref=""G:Whats-a-g"" /></summary></member>", x => x.SecondProperty)};

            string text = string.Empty;
            events.Subscribe(EventType.Warning, x => text = x);

            model.CombineToTypeHierarchy(members);

            Assert.AreEqual("Unsupported documentation member found: 'G:Whats-a-g'", text);
        }

        [Test]
        public void ShouldRaiseWarningOnUnexpectedKindInReferenceInType()
        {
            var events = new EventAggregator();
            var model = new DocumentationModelBuilder(RealParser, events);
            var members = new IDocumentationMember[] {Type<Second>(@"<member name=""T:Example.Second""><summary><see cref=""G:Whats-a-g"" /></summary></member>")};

            string text = string.Empty;
            events.Subscribe(EventType.Warning, x => text = x);

            model.CombineToTypeHierarchy(members);

            Assert.AreEqual("Unsupported documentation member found: 'G:Whats-a-g'", text);
        }
    }
}
