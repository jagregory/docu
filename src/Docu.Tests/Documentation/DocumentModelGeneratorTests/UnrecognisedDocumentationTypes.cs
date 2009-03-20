using Docu.Parsing.Model;
using Example;
using NUnit.Framework;

namespace Docu.Tests.Documentation.DocumentModelGeneratorTests
{
    [TestFixture]
    public class UnrecognisedDocumentationTypes : BaseDocumentModelGeneratorFixture
    {
        [Test]
        public void ShouldRaiseWarningOnUnexpectedKindInReferenceInType()
        {
            var warningRaised = false;
            var members = new IDocumentationMember[] { Type<Second>(@"<member name=""T:Example.Second""><summary><see cref=""G:Whats-a-g"" /></summary></member>") };

            model.CreationWarning += (sender, e) => { warningRaised = true; };
            model.Create(members);

            warningRaised.ShouldBeTrue();
        }

        [Test]
        public void ShouldHaveCorrectMessageInWarningWhenRaised()
        {
            var members = new IDocumentationMember[] { Type<Second>(@"<member name=""T:Example.Second""><summary><see cref=""G:Whats-a-g"" /></summary></member>") };
            var warningMessage = "";

            model.CreationWarning += (sender, e) => { warningMessage = e.Message; };
            model.Create(members);

            warningMessage.ShouldEqual("Unsupported documentation member found: 'G:Whats-a-g'");
        }

        [Test]
        public void ShouldRaiseWarningOnUnexpectedKindInReferenceInMethod()
        {
            var warningRaised = false;
            var members = new IDocumentationMember[] { Method<Second>(@"<member name=""M:Example.Second.SecondMethod""><summary><see cref=""G:Whats-a-g"" /></summary></member>", x => x.SecondMethod()) };

            model.CreationWarning += (sender, e) => { warningRaised = true; };
            model.Create(members);

            warningRaised.ShouldBeTrue();
        }

        [Test]
        public void ShouldRaiseWarningOnUnexpectedKindInReferenceInProperty()
        {
            var warningRaised = false;
            var members = new IDocumentationMember[] { Property<Second>(@"<member name=""P:Example.Second.SecondProperty""><summary><see cref=""G:Whats-a-g"" /></summary></member>", x => x.SecondProperty) };

            model.CreationWarning += (sender, e) => { warningRaised = true; };
            model.Create(members);

            warningRaised.ShouldBeTrue();
        }

        [Test]
        public void ShouldRaiseWarningOnUnexpectedKindInReferenceInEvent()
        {
            var warningRaised = false;
            var members = new IDocumentationMember[] { Event<Second>(@"<member name=""E:Example.Second.AnEvent""><summary><see cref=""G:Whats-a-g"" /></summary></member>", "AnEvent") };

            model.CreationWarning += (sender, e) => { warningRaised = true; };
            model.Create(members);

            warningRaised.ShouldBeTrue();
        }
    }
}