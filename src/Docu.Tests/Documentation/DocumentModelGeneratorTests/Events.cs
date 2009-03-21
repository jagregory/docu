using Docu.Parsing.Model;
using Example;
using NUnit.Framework;

namespace Docu.Tests.Documentation.DocumentModelGeneratorTests
{
    [TestFixture]
    public class Events : BaseDocumentModelGeneratorFixture
    {
        [Test]
        public void ShouldHaveEventsInTypes()
        {
            var members = new IDocumentationMember[]
            {
                Type<Second>(@"<member name=""T:Example.Second"" />"),  
                Event<Second>(@"<member name=""E:Example.Second.AnEvent"" />", "AnEvent"),
            };
            var namespaces = model.Create(members);
            var ev = typeof(Second).GetEvent("AnEvent");

            namespaces[0].Types[0].Events
                .ShouldContain(x => x.IsIdentifiedBy(Identifier.FromEvent(ev, typeof(Second))));
        }

        [Test]
        public void ShouldForceTypeIfOnlyEventDefined()
        {
            var members = new[] { Event<Second>(@"<member name=""E:Example.Second.AnEvent"" />", "AnEvent") };
            var namespaces = model.Create(members);

            namespaces[0].Name.ShouldEqual("Example");
            namespaces[0].Types.ShouldContain(x => x.IsIdentifiedBy(Identifier.FromType(typeof(Second))));
        }
    }
}