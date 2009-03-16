using System.Collections.Generic;
using DrDoc.Documentation;
using DrDoc.Documentation.Comments;
using DrDoc.Generation;
using DrDoc.Parsing.Model;
using Example;
using NUnit.Framework;

namespace DrDoc.Tests.Generation
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
                { "namespace.simple", "${Namespaces[0].Name}"},
                { "namespace.shortcut", "${Namespace.Name}"},
                { "namespace.linq", "<for each=\"var ns in Namespaces.Where(x => x.Name == 'Test')\">${ns.Name}</for>"},
                { "summary.simple", "<for each=\"var b in Namespaces[0].Types[0].Summary\">${b}</for>"},
                { "summary.flattened", "<var test=\"'xxx'\" />${WriteSummary(Namespaces[0].Types[0].Summary)}"},
                { "method.overload", "<for each=\"var method in Type.Methods\">${method.Name}(${OutputMethodParams(method)})</for>"}
            });
        }

        [Test]
        public void ShouldOutputSimpleNamespace()
        {
            var data = new OutputData { Namespaces = Namespaces("Example") };
            var content = generator.Convert("namespace.simple", data);

            content.ShouldEqual("Example");
        }

        [Test]
        public void ShouldOutputShortcutNamespace()
        {
            var data = new OutputData { Namespace = Namespace("Example") };
            var content = generator.Convert("namespace.shortcut", data);

            content.ShouldEqual("Example");
        }

        [Test]
        public void ShouldOutputNamespaceThatUsesLinq()
        {
            var data = new OutputData { Namespaces = Namespaces("Example", "Test") };
            var content = generator.Convert("namespace.linq", data);

            content.ShouldEqual("Test");
        }

        [Test]
        public void ShouldOutputSimpleSummary()
        {
            var ns = Namespace("Example");
            var type = Type<First>();
            type.Summary.Add(new InlineText("Hello world!"));
            ns.AddType(type);
            var data = new OutputData { Namespaces = new List<Namespace> { ns } };

            var content = generator.Convert("summary.simple", data);

            content.ShouldEqual("Hello world!");
        }

        [Test]
        public void ShouldOutputFlattenedSummary()
        {
            var ns = Namespace("Example");
            var type = Type<First>();
            type.Summary.Add(new InlineText("Hello world!"));
            ns.AddType(type);
            var data = new OutputData { Namespaces = new List<Namespace> { ns } };

            var content = generator.Convert("summary.flattened", data);

            content.ShouldEqual("Hello world!");
        }

        [Test]
        public void ShouldOutputOverloadedMethods()
        {
            var type = Type<First>();

            type.Methods.Add(new Method(Identifier.FromMethod(Method<ClassWithOverload>(x => x.Method()), typeof(ClassWithOverload)), ""));
            type.Methods.Add(new Method(Identifier.FromMethod(Method<ClassWithOverload>(x => x.Method(null)), typeof(ClassWithOverload)), ""));
            type.Methods[1].Parameters.Add(new MethodParameter("one", new ExternalReference(Identifier.FromType(typeof(string)), typeof(string))));
            
            var data = new OutputData { Type = type };
            var content = generator.Convert("method.overload", data);

            content.ShouldEqual("Method()Method(string one)"); // nasty, I know
        }
    }
}