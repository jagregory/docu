using System.Collections.Generic;
using System.Linq;
using Docu.Documentation;
using Docu.Documentation.Comments;
using Docu.Events;
using Docu.Parsing;
using Docu.Parsing.Model;
using Example;
using NUnit.Framework;

namespace Docu.Tests.Documentation.DocumentModelGeneratorTests
{
    [TestFixture]
    public class Values : BaseDocumentModelGeneratorFixture
    {
        [Test]
        public void ShouldHaveValueForMethod()
        {
            var model = new DocumentationModelBuilder(RealParser, new EventAggregator());
            var methods = new IDocumentationMember[]
                {
                    Type<ReturnMethodClass>(@"<member name=""T:Example.ReturnMethodClass"" />"),
                    Method<ReturnMethodClass>(@"<member name=""Example.ReturnMethodClass""><value>A string.</value></member>", x => x.Method())
                };
            IList<Namespace> namespaces = model.CombineToTypeHierarchy(methods);

            namespaces[0].Types[0].Methods[0].Value.Children.Count().ShouldEqual(1);
            ((InlineText) namespaces[0].Types[0].Methods[0].Value.Children.First()).Text.ShouldEqual("A string.");
        }

        [Test]
        public void ShouldHaveValueForProperty()
        {
            var model = new DocumentationModelBuilder(RealParser, new EventAggregator());
            var properties = new IDocumentationMember[]
                {
                    Type<Second>(@"<member name=""T:Example.Second"" />"),
                    Property<Second>(@"<member name=""P:Example.Second.SecondProperty""><value>The string representation.</value></member>", x => x.SecondProperty)
                };
            IList<Namespace> namespaces = model.CombineToTypeHierarchy(properties);

            namespaces[0].Types[0].Properties[0].Value.Children.Count().ShouldEqual(1);
            ((InlineText) namespaces[0].Types[0].Properties[0].Value.Children.First()).Text.ShouldEqual("The string representation.");
        }
    }
}
