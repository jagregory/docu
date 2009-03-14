using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DrDoc.Generation;
using DrDoc.IO;
using Example;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using TestFixture = NUnit.Framework.TestFixtureAttribute;
using Test = NUnit.Framework.TestAttribute;

namespace DrDoc.Tests
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
            var transformer = new TemplateTransformer(generator, writer, resolver);
            var namespaces = new DocNamespace[0];

            resolver.Stub(x => x.Resolve(null, null))
                .IgnoreArguments()
                .Return(new List<TemplateMatch> { new TemplateMatch("simple.htm", "simple.spark", new OutputData()) });

            transformer.Transform("simple.spark", namespaces);

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
            var transformer = new TemplateTransformer(generator, writer, resolver);
            var namespaces = new DocNamespace[0];

            resolver.Stub(x => x.Resolve(null, null))
                .IgnoreArguments()
                .Return(new List<TemplateMatch> { new TemplateMatch("simple.htm", "simple.spark", new OutputData()) });

            generator.Stub(x => x.Convert(null, null))
                .IgnoreArguments()
                .Return("content");

            transformer.Transform("simple.spark", namespaces);

            writer.AssertWasCalled(x => x.WriteFile("simple.htm", "content"));
        }

        [Test]
        public void GeneratesOutputForEachNamespaceFromTemplateWhenPatternUsed()
        {
            var generator = MockRepository.GenerateMock<IOutputGenerator>();
            var writer = MockRepository.GenerateStub<IOutputWriter>();
            var resolver = MockRepository.GenerateStub<IPatternTemplateResolver>();
            var transformer = new TemplateTransformer(generator, writer, resolver);
            var namespaces = Namespaces("One", "Two");

            resolver.Stub(x => x.Resolve(null, null))
                .IgnoreArguments()
                .Return(new List<TemplateMatch>
                {
                    new TemplateMatch("One.htm", "!namespace.spark", new OutputData()),
                    new TemplateMatch("Two.htm", "!namespace.spark", new OutputData())
                });

            transformer.Transform("!namespace.spark", namespaces);

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
            var transformer = new TemplateTransformer(generator, writer, resolver);
            var namespaces = Namespaces("One", "Two");

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

            transformer.Transform("!namespace.spark", namespaces);

            writer.AssertWasCalled(x => x.WriteFile("One.htm", "content"));
            writer.AssertWasCalled(x => x.WriteFile("Two.htm", "content"));
        }

        [Test]
        public void GeneratesOutputForEachTypeFromTemplateWhenPatternUsed()
        {
            var generator = MockRepository.GenerateMock<IOutputGenerator>();
            var writer = MockRepository.GenerateStub<IOutputWriter>();
            var resolver = MockRepository.GenerateStub<IPatternTemplateResolver>();
            var transformer = new TemplateTransformer(generator, writer, resolver);
            var namespaces = Namespaces("One", "Two");

            namespaces[0].AddType(Type<First>());
            namespaces[1].AddType(Type<Second>());

            resolver.Stub(x => x.Resolve(null, null))
                .IgnoreArguments()
                .Return(new List<TemplateMatch>
                {
                    new TemplateMatch("One.First.htm", "!type.spark", new OutputData()),
                    new TemplateMatch("Two.Second.htm", "!type.spark", new OutputData())
                });

            transformer.Transform("!type.spark", namespaces);

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
            var transformer = new TemplateTransformer(generator, writer, resolver);
            var namespaces = Namespaces("One", "Two");

            namespaces[0].AddType(Type<First>());
            namespaces[1].AddType(Type<Second>());

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

            transformer.Transform("!type.spark", namespaces);

            writer.AssertWasCalled(x => x.WriteFile("One.First.htm", "content"));
            writer.AssertWasCalled(x => x.WriteFile("Two.Second.htm", "content"));
        }

        [Test]
        public void TransformsTemplateInDirectoriesWithNamespacePattern()
        {
            var generator = MockRepository.GenerateStub<IOutputGenerator>();
            var writer = MockRepository.GenerateMock<IOutputWriter>();
            var resolver = MockRepository.GenerateStub<IPatternTemplateResolver>();
            var transformer = new TemplateTransformer(generator, writer, resolver);
            var namespaces = Namespaces("One", "Two");

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

            transformer.Transform("!namespace\\template.spark", namespaces);

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
            var transformer = new TemplateTransformer(generator, writer, resolver);
            var namespaces = Namespaces("One", "Two");

            namespaces[0].AddType(Type<First>());
            namespaces[1].AddType(Type<Second>());

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

            transformer.Transform("!type\\template.spark", namespaces);

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
            var transformer = new TemplateTransformer(generator, writer, resolver);
            var namespaces = Namespaces("One", "Two");

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

            transformer.Transform("!namespace\\test.spark", namespaces);

            writer.AssertWasCalled(x => x.WriteFile("One\\test.htm", "content"));
            writer.AssertWasCalled(x => x.WriteFile("Two\\test.htm", "content"));
        }

        [Test]
        public void TransformsTemplatesInDirectories()
        {
            var generator = MockRepository.GenerateStub<IOutputGenerator>();
            var writer = MockRepository.GenerateMock<IOutputWriter>();
            var resolver = MockRepository.GenerateStub<IPatternTemplateResolver>();
            var transformer = new TemplateTransformer(generator, writer, resolver);
            var namespaces = new DocNamespace[0];

            resolver.Stub(x => x.Resolve(null, null))
                .IgnoreArguments()
                .Return(new List<TemplateMatch> { new TemplateMatch("directory\\test.htm", "", new OutputData()) });
            generator.Stub(x => x.Convert(null, null))
                .IgnoreArguments()
                .Return("content");

            transformer.Transform("directory\\test.spark", namespaces);

            writer.AssertWasCalled(x => x.WriteFile("directory\\test.htm", "content"));
        }
    }
}
