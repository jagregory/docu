using System.Collections.Generic;
using Docu.Documentation;
using Docu.Generation;
using Docu.IO;
using Example;
using Rhino.Mocks;
using TestFixture = NUnit.Framework.TestFixtureAttribute;
using Test = NUnit.Framework.TestAttribute;

namespace Docu.Tests.Generation
{
    [TestFixture]
    public class TemplateTransformer_OutputDir_Tests : BaseFixture
    {
        [Test]
        public void WritesOutputToFile()
        {
            var generator = MockRepository.GenerateStub<IOutputGenerator>();
            var writer = MockRepository.GenerateMock<IOutputWriter>();
            var resolver = MockRepository.GenerateStub<IPatternTemplateResolver>();
            var transformer = new PageWriter(generator, writer, resolver);
            var namespaces = new Namespace[0];

            resolver.Stub(x => x.Resolve(null, null))
                .IgnoreArguments()
                .Return(new List<TemplateMatch> { new TemplateMatch("simple.htm", "simple.spark", new ViewData()) });

            generator.Stub(x => x.Convert(null, null))
                .IgnoreArguments()
                .Return("content");

            transformer.CreatePages("simple.spark", "output", namespaces);

            writer.AssertWasCalled(x => x.WriteFile("output\\simple.htm", "content"));
        }

        [Test]
        public void WritesOutputForEachNamespaceToFileWhenPatternUsed()
        {
            var generator = MockRepository.GenerateStub<IOutputGenerator>();
            var writer = MockRepository.GenerateMock<IOutputWriter>();
            var resolver = MockRepository.GenerateStub<IPatternTemplateResolver>();
            var transformer = new PageWriter(generator, writer, resolver);
            var namespaces = Namespaces("One", "Two");

            resolver.Stub(x => x.Resolve(null, null))
                .IgnoreArguments()
                .Return(new List<TemplateMatch>
                {
                    new TemplateMatch("One.htm", "!namespace.spark", new ViewData()),
                    new TemplateMatch("Two.htm", "!namespace.spark", new ViewData())
                });

            generator.Stub(x => x.Convert(null, null))
                .IgnoreArguments()
                .Return("content");

            transformer.CreatePages("!namespace.spark", "output", namespaces);

            writer.AssertWasCalled(x => x.WriteFile("output\\One.htm", "content"));
            writer.AssertWasCalled(x => x.WriteFile("output\\Two.htm", "content"));
        }

        [Test]
        public void WritesOutputForEachTypeToFileWhenPatternUsed()
        {
            var generator = MockRepository.GenerateStub<IOutputGenerator>();
            var writer = MockRepository.GenerateMock<IOutputWriter>();
            var resolver = MockRepository.GenerateStub<IPatternTemplateResolver>();
            var transformer = new PageWriter(generator, writer, resolver);
            var namespaces = Namespaces("One", "Two");

            Type<First>(namespaces[0]);
            Type<Second>(namespaces[1]);

            resolver.Stub(x => x.Resolve(null, null))
                .IgnoreArguments()
                .Return(new List<TemplateMatch>
                {
                    new TemplateMatch("One.First.htm", "!type.spark", new ViewData()),
                    new TemplateMatch("Two.Second.htm", "!type.spark", new ViewData())
                });

            generator.Stub(x => x.Convert(null, null))
                .IgnoreArguments()
                .Return("content");

            transformer.CreatePages("!type.spark", "output", namespaces);

            writer.AssertWasCalled(x => x.WriteFile("output\\One.First.htm", "content"));
            writer.AssertWasCalled(x => x.WriteFile("output\\Two.Second.htm", "content"));
        }

        [Test]
        public void TransformsTemplateInDirectoriesWithNamespacePattern()
        {
            var generator = MockRepository.GenerateStub<IOutputGenerator>();
            var writer = MockRepository.GenerateMock<IOutputWriter>();
            var resolver = MockRepository.GenerateStub<IPatternTemplateResolver>();
            var transformer = new PageWriter(generator, writer, resolver);
            var namespaces = Namespaces("One", "Two");

            resolver.Stub(x => x.Resolve(null, null))
                .IgnoreArguments()
                .Return(new List<TemplateMatch>
                {
                    new TemplateMatch("One\\template.htm", "!namespace\\template.spark", new ViewData()),
                    new TemplateMatch("Two\\template.htm", "!namespace\\template.spark", new ViewData())
                });

            writer.Stub(x => x.Exists(null))
                .IgnoreArguments()
                .Return(false);

            generator.Stub(x => x.Convert(null, null))
                .IgnoreArguments()
                .Return("content");

            transformer.CreatePages("!namespace\\template.spark", "output", namespaces);

            writer.AssertWasCalled(x => x.CreateDirectory("output\\One"));
            writer.AssertWasCalled(x => x.WriteFile("output\\One\\template.htm", "content"));
            writer.AssertWasCalled(x => x.CreateDirectory("output\\Two"));
            writer.AssertWasCalled(x => x.WriteFile("output\\Two\\template.htm", "content"));
        }

        [Test]
        public void TransformsTemplatesInDirectoriesWithTypePattern()
        {
            var generator = MockRepository.GenerateStub<IOutputGenerator>();
            var writer = MockRepository.GenerateMock<IOutputWriter>();
            var resolver = MockRepository.GenerateStub<IPatternTemplateResolver>();
            var transformer = new PageWriter(generator, writer, resolver);
            var namespaces = Namespaces("One", "Two");

            Type<First>(namespaces[0]);
            Type<Second>(namespaces[1]);

            resolver.Stub(x => x.Resolve(null, null))
                .IgnoreArguments()
                .Return(new List<TemplateMatch>
                {
                    new TemplateMatch("One.First\\template.htm", "!type\\template.spark", new ViewData()),
                    new TemplateMatch("Two.Second\\template.htm", "!type\\template.spark", new ViewData()),
                });

            writer.Stub(x => x.Exists(null))
                .IgnoreArguments()
                .Return(false);

            generator.Stub(x => x.Convert(null, null))
                .IgnoreArguments()
                .Return("content");

            transformer.CreatePages("!type\\template.spark", "output", namespaces);

            writer.AssertWasCalled(x => x.CreateDirectory("output\\One.First"));
            writer.AssertWasCalled(x => x.WriteFile("output\\One.First\\template.htm", "content"));
            writer.AssertWasCalled(x => x.CreateDirectory("output\\Two.Second"));
            writer.AssertWasCalled(x => x.WriteFile("output\\Two.Second\\template.htm", "content"));
        }

        [Test]
        public void TransformsTemplatesWithinPatternDirectories()
        {
            var generator = MockRepository.GenerateStub<IOutputGenerator>();
            var writer = MockRepository.GenerateMock<IOutputWriter>();
            var resolver = MockRepository.GenerateStub<IPatternTemplateResolver>();
            var transformer = new PageWriter(generator, writer, resolver);
            var namespaces = Namespaces("One", "Two");

            resolver.Stub(x => x.Resolve(null, null))
                .IgnoreArguments()
                .Return(new List<TemplateMatch>
                {
                    new TemplateMatch("One\\test.htm", "", new ViewData()),
                    new TemplateMatch("Two\\test.htm", "", new ViewData()),
                });
            generator.Stub(x => x.Convert(null, null))
                .IgnoreArguments()
                .Return("content");

            transformer.CreatePages("!namespace\\test.spark", "output", namespaces);

            writer.AssertWasCalled(x => x.WriteFile("output\\One\\test.htm", "content"));
            writer.AssertWasCalled(x => x.WriteFile("output\\Two\\test.htm", "content"));
        }

        [Test]
        public void TransformsTemplatesInDirectories()
        {
            var generator = MockRepository.GenerateStub<IOutputGenerator>();
            var writer = MockRepository.GenerateMock<IOutputWriter>();
            var resolver = MockRepository.GenerateStub<IPatternTemplateResolver>();
            var transformer = new PageWriter(generator, writer, resolver);
            var namespaces = new Namespace[0];

            resolver.Stub(x => x.Resolve(null, null))
                .IgnoreArguments()
                .Return(new List<TemplateMatch> { new TemplateMatch("directory\\test.htm", "", new ViewData()) });
            generator.Stub(x => x.Convert(null, null))
                .IgnoreArguments()
                .Return("content");

            transformer.CreatePages("directory\\test.spark", "output", namespaces);

            writer.AssertWasCalled(x => x.WriteFile("output\\directory\\test.htm", "content"));
        }

        [Test]
        public void when_template_directory_set_exclude_directory_from_output()
        {
            var generator = MockRepository.GenerateStub<IOutputGenerator>();
            var writer = MockRepository.GenerateMock<IOutputWriter>();
            var resolver = MockRepository.GenerateStub<IPatternTemplateResolver>();
            var transformer = new PageWriter(generator, writer, resolver);
            var namespaces = new Namespace[0];

            resolver.Stub(x => x.Resolve(null, null))
                .IgnoreArguments()
                .Return(new List<TemplateMatch> { new TemplateMatch("someTemplatePath\\simple.htm", "someTemplatePath\\simple.spark", new ViewData()) });

            generator.Stub(x => x.Convert(null, null))
                .IgnoreArguments()
                .Return("content");

            transformer.SetTemplatePath("someTemplatePath");
            transformer.CreatePages("someTemplatePath\\simple.spark", "output", namespaces);

            writer.AssertWasCalled(x => x.WriteFile("output\\simple.htm", "content"));
        }

        [Test]
        public void when_template_directory_set_propagate_change_to_the_output_generator()
        {
            var generator = MockRepository.GenerateStub<IOutputGenerator>();
            var writer = MockRepository.GenerateMock<IOutputWriter>();
            var resolver = MockRepository.GenerateStub<IPatternTemplateResolver>();
            var transformer = new PageWriter(generator, writer, resolver);

            transformer.SetTemplatePath("someTemplatePath");
            generator.AssertWasCalled(g => g.SetTemplatePath("someTemplatePath"));
        }

    }
}