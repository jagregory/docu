using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DrDoc.Generation;
using DrDoc.IO;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using TestFixture = NUnit.Framework.TestFixtureAttribute;
using Test = NUnit.Framework.TestAttribute;

namespace DrDoc.Tests
{
    [TestFixture]
    public class TemplateTransformerTests
    {
        [Test]
        public void GeneratesOutputFromTemplate()
        {
            var generator = MockRepository.GenerateMock<IOutputGenerator>();
            var writer = MockRepository.GenerateStub<IOutputWriter>();
            var transformer = new TemplateTransformer(generator, writer);
            var namespaces = new DocNamespace[0];

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
            var transformer = new TemplateTransformer(generator, writer);
            var namespaces = new DocNamespace[0];

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
            var transformer = new TemplateTransformer(generator, writer);
            var namespaces = new[] { new DocNamespace("One"), new DocNamespace("Two")};

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
            var transformer = new TemplateTransformer(generator, writer);
            var namespaces = new[] { new DocNamespace("One"), new DocNamespace("Two") };

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
            var transformer = new TemplateTransformer(generator, writer);
            var namespaces = new[] { new DocNamespace("One"), new DocNamespace("Two") };

            namespaces[0].AddType(new DocType("TypeOne"));
            namespaces[1].AddType(new DocType("TypeTwo"));

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
            var transformer = new TemplateTransformer(generator, writer);
            var namespaces = new[] { new DocNamespace("One"), new DocNamespace("Two") };

            namespaces[0].AddType(new DocType("TypeOne"));
            namespaces[1].AddType(new DocType("TypeTwo"));

            generator.Stub(x => x.Convert(null, null))
                .IgnoreArguments()
                .Return("content");

            transformer.Transform("!type.spark", namespaces);

            writer.AssertWasCalled(x => x.WriteFile("One.TypeOne.htm", "content"));
            writer.AssertWasCalled(x => x.WriteFile("Two.TypeTwo.htm", "content"));
        }
    }
}
