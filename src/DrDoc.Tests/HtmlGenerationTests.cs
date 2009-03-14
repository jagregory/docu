using System.Collections.Generic;
using DrDoc.Generation;
using NUnit.Framework;

namespace DrDoc.Tests
{
    [TestFixture]
    public class HtmlGenerationTests
    {
        private HtmlGenerator generator;

        [SetUp]
        public void CreateGenerator()
        {
            generator = new HtmlGenerator(new Dictionary<string, string>
            {
                { "namespace.simple", "${Namespaces[0].Name}"},
                { "namespace.shortcut", "${Namespace.Name}"},
                { "summary.simple", "<for each=\"var b in Namespaces[0].Types[0].Summary\">${b}</for>"},
                { "summary.flattened", "<var test=\"'xxx'\" />${flatten(Namespaces[0].Types[0].Summary)}"},
            });
        }

        [Test]
        public void ShouldOutputSimpleNamespace()
        {
            var data = new OutputData { Namespaces = new List<DocNamespace> { new DocNamespace("Example") } };
            var content = generator.Convert("namespace.simple", data);

            content.ShouldEqual("Example");
        }

        [Test]
        public void ShouldOutputShortcutNamespace()
        {
            var data = new OutputData { Namespace = new DocNamespace("Example") };
            var content = generator.Convert("namespace.shortcut", data);

            content.ShouldEqual("Example");
        }

        [Test]
        public void ShouldOutputSimpleSummary()
        {
            var ns = new DocNamespace("Example");
            var type = new DocType("First");
            type.Summary.Add(new DocTextBlock("Hello world!"));
            ns.AddType(type);
            var data = new OutputData { Namespaces = new List<DocNamespace> { ns } };

            var content = generator.Convert("summary.simple", data);

            content.ShouldEqual("Hello world!");
        }

        [Test]
        public void ShouldOutputFlattenedSummary()
        {
            var ns = new DocNamespace("Example");
            var type = new DocType("First");
            type.Summary.Add(new DocTextBlock("Hello world!"));
            ns.AddType(type);
            var data = new OutputData { Namespaces = new List<DocNamespace> { ns } };

            var content = generator.Convert("summary.flattened", data);

            content.ShouldEqual("Hello world!");
        }
    }
}