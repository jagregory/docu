using System.Linq;
using Docu.Events;
using Docu.Parsing;
using Docu.Parsing.Model;
using Example;
using NUnit.Framework;

namespace Docu.Tests.Documentation.DocumentModelGeneratorTests
{
    [TestFixture]
    public class Examples : BaseDocumentModelGeneratorFixture
    {
        [Test]
        public void ShouldHaveExampleForMethod()
        {
            var model = new DocumentationModelBuilder(RealParser, new EventAggregator());
            var members = new IDocumentationMember[]
                {
                    Type<Second>(@"<member name=""T:Example.Second"" />"),
                    Method<Second>("<member name=\"M:Example.Second.SecondMethod2(System.String,System.Int32)\"><example>\r\n                    void Something()\r\n                    {\r\n                      return;\r\n                    }\r\n                  </example></member>", x => x.SecondMethod2(null, 0))
                };
            var namespaces = model.CombineToTypeHierarchy(members);
            var method = namespaces.Single().Classes.Single().Methods.Single();

            method.Example.ShouldNotBeNull();
            method.Example.ShouldMatchStructure(com => com.InlineText("void Something()\r\n{\r\n  return;\r\n}"));
        }

        [Test]
        public void ShouldHaveExampleForProperty()
        {
            var model = new DocumentationModelBuilder(RealParser, new EventAggregator());
            var members = new IDocumentationMember[]
                {
                    Type<Second>(@"<member name=""T:Example.Second"" />"),
                    Property<Second>("<member name=\"P:Example.Second.SecondProperty\"><example>\r\n                    void Something()\r\n                    {\r\n                      return;\r\n                    }\r\n                  </example></member>", x => x.SecondProperty)
                };
            var namespaces = model.CombineToTypeHierarchy(members);
            var property = namespaces.Single().Classes.Single().Properties.Single();

            property.Example.ShouldNotBeNull();
            property.Example.ShouldMatchStructure(com => com.InlineText("void Something()\r\n{\r\n  return;\r\n}"));
        }

        [Test]
        public void ShouldHaveExampleForField()
        {
            var model = new DocumentationModelBuilder(RealParser, new EventAggregator());
            var members = new IDocumentationMember[]
                {
                    Type<Second>(@"<member name=""T:Example.Second"" />"),
                    Field<Second>("<member name=\"F:Example.Second.aField\"><example>\r\n                    void Something()\r\n                    {\r\n                      return;\r\n                    }\r\n                  </example></member>", x => x.aField)
                };
            var namespaces = model.CombineToTypeHierarchy(members);
            var field = namespaces.Single().Classes.Single().Fields.Single();

            field.Example.ShouldNotBeNull();
            field.Example.ShouldMatchStructure(com => com.InlineText("void Something()\r\n{\r\n  return;\r\n}"));
        }

        [Test]
        public void ShouldHaveExampleForEvent()
        {
            var model = new DocumentationModelBuilder(RealParser, new EventAggregator());
            var members = new IDocumentationMember[]
                {
                    Type<Second>(@"<member name=""T:Example.Second"" />"),
                    Event<Second>("<member name=\"E:Example.Second.AnEvent\"><example>\r\n                    void Something()\r\n                    {\r\n                      return;\r\n                    }\r\n                  </example></member>", "AnEvent")
                };
            var namespaces = model.CombineToTypeHierarchy(members);
            var @event = namespaces.Single().Classes.Single().Events.Single();

            @event.Example.ShouldNotBeNull();
            @event.Example.ShouldMatchStructure(com => com.InlineText("void Something()\r\n{\r\n  return;\r\n}"));
        }

        [Test]
        public void ShouldHaveExampleForType()
        {
            var model = new DocumentationModelBuilder(RealParser, new EventAggregator());
            var members = new IDocumentationMember[]
                {
                    Type<Second>("<member name=\"T:Example.Second\"><example>\r\n                    void Something()\r\n                    {\r\n                      return;\r\n                    }\r\n                  </example></member>")
                };
            var namespaces = model.CombineToTypeHierarchy(members);
            var type = namespaces.Single().Classes.Single();

            type.Example.ShouldNotBeNull();
            type.Example.ShouldMatchStructure(com => com.InlineText("void Something()\r\n{\r\n  return;\r\n}"));
        }
    }
}
