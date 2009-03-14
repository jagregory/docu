using System.Collections.Generic;
using DrDoc.Associations;
using DrDoc.Generation;
using Example;
using NUnit.Framework;

namespace DrDoc.Tests
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
                { "namespace.simple", "${Namespaces[0].Name.ToString()}"},
                { "namespace.shortcut", "${Namespace.Name.ToString()}"},
                { "summary.simple", "<for each=\"var b in Namespaces[0].Types[0].Summary\">${b}</for>"},
                { "summary.flattened", "<var test=\"'xxx'\" />${flatten(Namespaces[0].Types[0].Summary)}"},
                { "method.overload", "<for each=\"var method in Type.Methods\">${method.Name.ToString()}(${OutputMethodParams(method)})</for>"}
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
        public void ShouldOutputSimpleSummary()
        {
            var ns = Namespace("Example");
            var type = Type<First>();
            type.Summary.Add(new DocTextBlock("Hello world!"));
            ns.AddType(type);
            var data = new OutputData { Namespaces = new List<DocNamespace> { ns } };

            var content = generator.Convert("summary.simple", data);

            content.ShouldEqual("Hello world!");
        }

        [Test]
        public void ShouldOutputFlattenedSummary()
        {
            var ns = Namespace("Example");
            var type = Type<First>();
            type.Summary.Add(new DocTextBlock("Hello world!"));
            ns.AddType(type);
            var data = new OutputData { Namespaces = new List<DocNamespace> { ns } };

            var content = generator.Convert("summary.flattened", data);

            content.ShouldEqual("Hello world!");
        }

        [Test]
        public void ShouldOutputOverloadedMethods()
        {
            var type = Type<First>();

            type.Methods.Add(new DocMethod(MemberName.FromMethod(Method<ClassWithOverload>(x => x.Method()), typeof(ClassWithOverload)), ""));
            type.Methods.Add(new DocMethod(MemberName.FromMethod(Method<ClassWithOverload>(x => x.Method(null)), typeof(ClassWithOverload)), ""));
            type.Methods[1].Parameters.Add(new DocParameter("one", new ExternalReference(MemberName.FromType(typeof(string)))));
            
            var data = new OutputData { Type = type };
            var content = generator.Convert("method.overload", data);

            content.ShouldEqual("Method()Method(String one)"); // nasty, I know
        }
    }
}