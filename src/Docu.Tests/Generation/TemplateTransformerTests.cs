using System.Collections.Generic;
using Docu.Documentation;
using Docu.Generation;
using Docu.IO;
using Docu.Documentation;
using Docu.Generation;
using Docu.IO;
using Example;
using NUnit.Framework;
using Rhino.Mocks;
using Is=Rhino.Mocks.Constraints.Is;

namespace Docu.Tests.Generation
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
            var assemblies = new AssemblyDoc[0];

            resolver.Stub(x => x.Resolve(null, null))
                .IgnoreArguments()
                .Return(new List<TemplateMatch> { new TemplateMatch("simple.htm", "simple.spark", new OutputData()) });

            transformer.CreatePages("simple.spark", "", assemblies);

            generator.AssertWasCalled(
                x => x.Convert(null, null),
                x => x.Constraints(Is.Equal("simple.spark"), Is.Anything()));
        }

        [Test]
        public void WritesOutputToFile()
        {
            var generator = MockRepository.GenerateStub<IOutputGenerator>();
            var writer = MockRepository.GenerateMock<IOutputWriter>();
            var resolver = MockRepository.GenerateStub<IPatternTemplateResolver>();
            var transformer = new PageWriter(generator, writer, resolver);
            var assemblies = new AssemblyDoc[0];

            resolver.Stub(x => x.Resolve(null, null))
                .IgnoreArguments()
                .Return(new List<TemplateMatch> { new TemplateMatch("simple.htm", "simple.spark", new OutputData()) });

            generator.Stub(x => x.Convert(null, null))
                .IgnoreArguments()
                .Return("content");

            transformer.CreatePages("simple.spark", "", assemblies);

            writer.AssertWasCalled(x => x.WriteFile("simple.htm", "content"));
        }

        [Test]
        public void GeneratesOutputForEachNamespaceFromTemplateWhenPatternUsed()
        {
            var generator = MockRepository.GenerateMock<IOutputGenerator>();
            var writer = MockRepository.GenerateStub<IOutputWriter>();
            var resolver = MockRepository.GenerateStub<IPatternTemplateResolver>();
            var transformer = new PageWriter(generator, writer, resolver);
            var assemblies = new[] { AssemblyNamespaces("One", "Two") };

            resolver.Stub(x => x.Resolve(null, null))
                .IgnoreArguments()
                .Return(new List<TemplateMatch>
                {
                    new TemplateMatch("One.htm", "!namespace.spark", new OutputData()),
                    new TemplateMatch("Two.htm", "!namespace.spark", new OutputData())
                });

            transformer.CreatePages("!namespace.spark", "", assemblies);

            generator.AssertWasCalled(
                x => x.Convert(null, null),
                x => x.Constraints(Is.Equal("!namespace.spark"), Is.Anything())
                         .Repeat.Twice());
        }

        [Test]
        public void WritesOutputForEachNamespaceToFileWhenPatternUsed()
        {
            var generator = MockRepository.GenerateStub<IOutputGenerator>();
            var writer = MockRepository.GenerateMock<IOutputWriter>();
            var resolver = MockRepository.GenerateStub<IPatternTemplateResolver>();
            var transformer = new PageWriter(generator, writer, resolver);
            var assemblies = new[] { AssemblyNamespaces("One", "Two") };

            resolver.Stub(x => x.Resolve(null, null))
                .IgnoreArguments()
                .Return(new List<TemplateMatch>
                {
                    new TemplateMatch("One.htm", "!namespace.spark", new OutputData()),
                    new TemplateMatch("Two.htm", "!namespace.spark", new OutputData())
                });

            generator.Stub(x => x.Convert(null, null))
                .IgnoreArguments()
                .Return("content");

            transformer.CreatePages("!namespace.spark", "", assemblies);

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
            var assemblies = new[] { AssemblyNamespaces("Assembly", "One", "Two") };

            Type<First>(assemblies[0].Namespaces[0]);
            Type<Second>(assemblies[0].Namespaces[1]);

            resolver.Stub(x => x.Resolve(null, null))
                .IgnoreArguments()
                .Return(new List<TemplateMatch>
                {
                    new TemplateMatch("One.First.htm", "!type.spark", new OutputData()),
                    new TemplateMatch("Two.Second.htm", "!type.spark", new OutputData())
                });

            transformer.CreatePages("!type.spark", "", assemblies);

            generator.AssertWasCalled(
                x => x.Convert(null, null),
                x => x.Constraints(Is.Equal("!type.spark"), Is.Anything())
                         .Repeat.Twice());
        }

        [Test]
        public void WritesOutputForEachTypeToFileWhenPatternUsed()
        {
            var generator = MockRepository.GenerateStub<IOutputGenerator>();
            var writer = MockRepository.GenerateMock<IOutputWriter>();
            var resolver = MockRepository.GenerateStub<IPatternTemplateResolver>();
            var transformer = new PageWriter(generator, writer, resolver);
            var assemblies = new[] { AssemblyNamespaces("Assembly", "One", "Two") };

            Type<First>(assemblies[0].Namespaces[0]);
            Type<Second>(assemblies[0].Namespaces[1]);

            resolver.Stub(x => x.Resolve(null, null))
                .IgnoreArguments()
                .Return(new List<TemplateMatch>
                {
                    new TemplateMatch("One.First.htm", "!type.spark", new OutputData()),
                    new TemplateMatch("Two.Second.htm", "!type.spark", new OutputData())
                });

            generator.Stub(x => x.Convert(null, null))
                .IgnoreArguments()
                .Return("content");

            transformer.CreatePages("!type.spark", "", assemblies);

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
            var assemblies = new[] { AssemblyNamespaces("Assembly", "One", "Two") };

            resolver.Stub(x => x.Resolve(null, null))
                .IgnoreArguments()
                .Return(new List<TemplateMatch>
                {
                    new TemplateMatch("One\\template.htm", "!namespace\\template.spark", new OutputData()),
                    new TemplateMatch("Two\\template.htm", "!namespace\\template.spark", new OutputData())
                });

            writer.Stub(x => x.Exists(null))
                .IgnoreArguments()
                .Return(false);

            generator.Stub(x => x.Convert(null, null))
                .IgnoreArguments()
                .Return("content");

            transformer.CreatePages("!namespace\\template.spark", "", assemblies);

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
            var assemblies = new[] { AssemblyNamespaces("Assembly", "One", "Two") };

            Type<First>(assemblies[0].Namespaces[0]);
            Type<Second>(assemblies[0].Namespaces[1]);

            resolver.Stub(x => x.Resolve(null, null))
                .IgnoreArguments()
                .Return(new List<TemplateMatch>
                {
                    new TemplateMatch("One.First\\template.htm", "!type\\template.spark", new OutputData()),
                    new TemplateMatch("Two.Second\\template.htm", "!type\\template.spark", new OutputData()),
                });

            writer.Stub(x => x.Exists(null))
                .IgnoreArguments()
                .Return(false);

            generator.Stub(x => x.Convert(null, null))
                .IgnoreArguments()
                .Return("content");

            transformer.CreatePages("!type\\template.spark", "", assemblies);

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
            var assemblies = new[] { AssemblyNamespaces("One", "Two") };

            resolver.Stub(x => x.Resolve(null, null))
                .IgnoreArguments()
                .Return(new List<TemplateMatch>
                {
                    new TemplateMatch("One\\test.htm", "", new OutputData()),
                    new TemplateMatch("Two\\test.htm", "", new OutputData()),
                });
            generator.Stub(x => x.Convert(null, null))
                .IgnoreArguments()
                .Return("content");

            transformer.CreatePages("!namespace\\test.spark", "", assemblies);

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
            var assemblies = new AssemblyDoc[0];

            resolver.Stub(x => x.Resolve(null, null))
                .IgnoreArguments()
                .Return(new List<TemplateMatch> { new TemplateMatch("directory\\test.htm", "", new OutputData()) });
            generator.Stub(x => x.Convert(null, null))
                .IgnoreArguments()
                .Return("content");

            transformer.CreatePages("directory\\test.spark", "", assemblies);

            writer.AssertWasCalled(x => x.WriteFile("directory\\test.htm", "content"));
        }
    }
}