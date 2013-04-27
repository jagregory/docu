using System.Collections.Generic;
using Docu.Documentation;
using Docu.Output;
using Docu.IO;
using Example;
using NUnit.Framework;
using Rhino.Mocks;

namespace Docu.Tests.Output
{
    [TestFixture]
    public class TemplateTransformerTests : BaseFixture
    {
        [Test]
        public void GeneratesOutputFromTemplate()
        {
            var generator = MockRepository.GenerateMock<IOutputGenerator>();
            var writer = MockRepository.GenerateStub<IOutputWriter>();
            var resolver = MockRepository.GenerateStub<IPatternTemplateResolver>();
            var transformer = new PageWriter(generator, writer, resolver);
            var namespaces = new Namespace[0];

            resolver.Stub(x => x.Resolve(null, null))
                .IgnoreArguments()
                .Return(new List<TemplateMatch> { new TemplateMatch("simple.htm", "simple.spark", new ViewData()) });

            transformer.CreatePages("simple.spark", "", namespaces);

            generator.AssertWasCalled(
                x => x.Convert(Arg.Is("simple.spark"), Arg<ViewData>.Is.Anything, Arg<string>.Is.Anything));
        }

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

            generator.Stub(x => x.Convert(null, null, null))
                .IgnoreArguments()
                .Return("content");

            transformer.CreatePages("simple.spark", "", namespaces);

            writer.AssertWasCalled(x => x.WriteFile("simple.htm", "content"));
        }

        [Test]
        public void GeneratesOutputForEachNamespaceFromTemplateWhenPatternUsed()
        {
            var generator = MockRepository.GenerateMock<IOutputGenerator>();
            var writer = MockRepository.GenerateStub<IOutputWriter>();
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

            transformer.CreatePages("!namespace.spark", "", namespaces);

            generator.AssertWasCalled(
                x => x.Convert(Arg.Is("!namespace.spark"), Arg<ViewData>.Is.Anything, Arg<string>.Is.Anything),
                x => x.Repeat.Twice());
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

            generator.Stub(x => x.Convert(null, null, null))
                .IgnoreArguments()
                .Return("content");

            transformer.CreatePages("!namespace.spark", "", namespaces);

            writer.AssertWasCalled(x => x.WriteFile("One.htm", "content"));
            writer.AssertWasCalled(x => x.WriteFile("Two.htm", "content"));
        }

        [Test]
        public void GeneratesOutputForEachTypeFromTemplateWhenPatternUsed()
        {
            var generator = MockRepository.GenerateMock<IOutputGenerator>();
            var writer = MockRepository.GenerateStub<IOutputWriter>();
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

            transformer.CreatePages("!type.spark", "", namespaces);

            generator.AssertWasCalled(
                x => x.Convert(Arg.Is("!type.spark"), Arg<ViewData>.Is.Anything, Arg<string>.Is.Anything),
                x => x.Repeat.Twice());
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

            generator.Stub(x => x.Convert(null, null, null))
                .IgnoreArguments()
                .Return("content");

            transformer.CreatePages("!type.spark", "", namespaces);

            writer.AssertWasCalled(x => x.WriteFile("One.First.htm", "content"));
            writer.AssertWasCalled(x => x.WriteFile("Two.Second.htm", "content"));
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

            generator.Stub(x => x.Convert(null, null, null))
                .IgnoreArguments()
                .Return("content");

            transformer.CreatePages("!namespace\\template.spark", "", namespaces);

            writer.AssertWasCalled(x => x.CreateDirectory("One"));
            writer.AssertWasCalled(x => x.WriteFile("One\\template.htm", "content"));
            writer.AssertWasCalled(x => x.CreateDirectory("Two"));
            writer.AssertWasCalled(x => x.WriteFile("Two\\template.htm", "content"));
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

            generator.Stub(x => x.Convert(null, null, null))
                .IgnoreArguments()
                .Return("content");

            transformer.CreatePages("!type\\template.spark", "", namespaces);

            writer.AssertWasCalled(x => x.CreateDirectory("One.First"));
            writer.AssertWasCalled(x => x.WriteFile("One.First\\template.htm", "content"));
            writer.AssertWasCalled(x => x.CreateDirectory("Two.Second"));
            writer.AssertWasCalled(x => x.WriteFile("Two.Second\\template.htm", "content"));
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
            generator.Stub(x => x.Convert(null, null, null))
                .IgnoreArguments()
                .Return("content");

            transformer.CreatePages("!namespace\\test.spark", "", namespaces);

            writer.AssertWasCalled(x => x.WriteFile("One\\test.htm", "content"));
            writer.AssertWasCalled(x => x.WriteFile("Two\\test.htm", "content"));
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
            generator.Stub(x => x.Convert(null, null, null))
                .IgnoreArguments()
                .Return("content");

            transformer.CreatePages("directory\\test.spark", "", namespaces);

            writer.AssertWasCalled(x => x.WriteFile("directory\\test.htm", "content"));
        }
    }
}