using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DrDoc.Documentation;
using DrDoc.Generation;
using DrDoc.IO;
using Example;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using TestFixture = NUnit.Framework.TestFixtureAttribute;
using Test = NUnit.Framework.TestAttribute;

namespace DrDoc.Tests.Generation
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
                .Return(new List<TemplateMatch> { new TemplateMatch("simple.htm", "simple.spark", new OutputData()) });

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
                    new TemplateMatch("One.htm", "!namespace.spark", new OutputData()),
                    new TemplateMatch("Two.htm", "!namespace.spark", new OutputData())
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
                    new TemplateMatch("One\\template.htm", "!namespace\\template.spark", new OutputData()),
                    new TemplateMatch("Two\\template.htm", "!namespace\\template.spark", new OutputData())
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
                    new TemplateMatch("One\\test.htm", "", new OutputData()),
                    new TemplateMatch("Two\\test.htm", "", new OutputData()),
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
                .Return(new List<TemplateMatch> { new TemplateMatch("directory\\test.htm", "", new OutputData()) });
            generator.Stub(x => x.Convert(null, null))
                .IgnoreArguments()
                .Return("content");

            transformer.CreatePages("directory\\test.spark", "output", namespaces);

            writer.AssertWasCalled(x => x.WriteFile("output\\directory\\test.htm", "content"));
        }
    }
}