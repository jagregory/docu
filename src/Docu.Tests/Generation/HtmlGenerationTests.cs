using System.Collections.Generic;
using Docu.Documentation;
using Docu.Documentation.Comments;
using Docu.Generation;
using Docu.Parsing.Model;
using Example;
using NUnit.Framework;

namespace Docu.Tests.Generation
{
    [TestFixture]
    public class HtmlGenerationTests : BaseFixture
    {
        private HtmlGenerator generator;

        [SetUp]
        public void CreateGenerator()
        {
            generator = new HtmlGenerator(new Dictionary<string, string>
            {
                { "assemblies", "${Assemblies[0].FullName}"},
                { "assembly", "${Assembly.Name}"},

                { "namespace.simple", "${Namespaces[0].Name}"},
                { "namespace.shortcut", "${Namespace.Name}"},
                { "namespace.linq", "<for each=\"var ns in Namespaces.Where(x => x.Name == 'Test')\">${ns.Name}</for>"},
                { "summary.simple", "${WriteSummary(Type.Summary)}"},
                { "method.overload", "<for each=\"var method in Type.Methods\">${method.Name}(${OutputMethodParams(method)})</for>"},
                { "method.returnType", "<for each=\"var method in Type.Methods\">${method.ReturnType.PrettyName}</for>"},
                { "property.returnType", "<for each=\"var property in Type.Properties\">${property.ReturnType.PrettyName}</for>"},
            });
        }

        [Test]
        public void ShouldOutputAssemblies()
        {
            var a1 = new AssemblyDoc("One");
            var a2 = new AssemblyDoc("Two");
            var data = new ViewData { Assemblies = new[] { typeof(ViewData).Assembly } };
            var content = generator.Convert("assemblies", data);

            content.ShouldEqual(typeof(ViewData).Assembly.FullName);
        }

        [Test]
        public void ShouldOutputSimpleNamespace()
        {
            var data = new ViewData { Namespaces = Namespaces("Example") };
            var content = generator.Convert("namespace.simple", data);

            content.ShouldEqual("Example");
        }

        [Test]
        public void ShouldOutputShortcutNamespace()
        {
            var data = new ViewData { Namespace = Namespace("Example") };
            var content = generator.Convert("namespace.shortcut", data);

            content.ShouldEqual("Example");
        }

        [Test]
        public void ShouldOutputNamespaceThatUsesLinq()
        {
            var data = new ViewData { Namespaces = Namespaces("Example", "Test") };
            var content = generator.Convert("namespace.linq", data);

            content.ShouldEqual("Test");
        }

        [Test]
        public void ShouldOutputSimpleSummary()
        {
            var ns = Namespace("Example");
            var type = Type<First>(ns);
            type.Summary.Add(new InlineText("Hello world!"));
            var data = new ViewData { Type = type };

            var content = generator.Convert("summary.simple", data);

            content.ShouldEqual("Hello world!");
        }

        [Test]
        public void ShouldOutputOverloadedMethods()
        {
            var ns = Namespace("Example");
            var type = Type<ClassWithOverload>(ns);
            var parameterType = DeclaredType.Unresolved(Identifier.FromType(typeof(string)), typeof(string), Namespace("System"));

            type.Methods.Add(new Docu.Documentation.Method(Identifier.FromMethod(Method<ClassWithOverload>(x => x.Method()), typeof(ClassWithOverload))));
            type.Methods.Add(new Docu.Documentation.Method(Identifier.FromMethod(Method<ClassWithOverload>(x => x.Method(null)), typeof(ClassWithOverload))));
            type.Methods[1].Parameters.Add(new MethodParameter("one", parameterType));
            
            var data = new ViewData { Type = type };
            var content = generator.Convert("method.overload", data);

            content.ShouldEqual("Method()Method(string one)"); // nasty, I know
        }

        [Test]
        public void ShouldOutputMethodReturnType()
        {
            var ns = Namespace("Example");
            var type = Type<ReturnMethodClass>(ns);
            var returnType = DeclaredType.Unresolved(Identifier.FromType(typeof(string)), typeof(string), Namespace("System"));

            type.Methods.Add(new Method(Identifier.FromMethod(Method<ReturnMethodClass>(x => x.Method()), typeof(ReturnMethodClass))));
            type.Methods[0].ReturnType = returnType;

            var data = new ViewData { Type = type };
            var content = generator.Convert("method.returnType", data);

            content.ShouldEqual("string");
        }

        [Test]
        public void ShouldOutputPropertyReturnType()
        {
            var ns = Namespace("Example");
            var type = Type<PropertyType>(ns);
            var returnType = DeclaredType.Unresolved(Identifier.FromType(typeof(string)), typeof(string), Namespace("System"));

            type.Properties.Add(new Property(Identifier.FromProperty(Property<PropertyType>(x => x.Property), typeof(PropertyType))));
            type.Properties[0].ReturnType = returnType;

            var data = new ViewData { Type = type };
            var content = generator.Convert("property.returnType", data);

            content.ShouldEqual("string");
        }
    }
}