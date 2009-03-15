using System.IO;
using DrDoc.Documentation;
using DrDoc.Generation;
using NUnit.Framework;
using Rhino.Mocks;

namespace DrDoc.Tests.Generation
{
    [TestFixture]
    public class BulkTransformer_Directory_Tests
    {
        private string directory;
        private string directory_sub;
        private string directory_namespace;
        private string directory_oneSpark;
        private string directory_twoSpark;
        private string directory_namespaceSpark;
        private string directory_sub_oneSpark;
        private string directory_namespace_oneSpark;

        [SetUp]
        public void CreateDirectory()
        {
            directory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            directory_oneSpark = Path.Combine(directory, "one.spark");
            directory_twoSpark = Path.Combine(directory, "two.spark");
            directory_namespaceSpark = Path.Combine(directory, "!namespace.spark");
            directory_sub = Path.Combine(directory, "sub");
            directory_sub_oneSpark = Path.Combine(directory_sub, "one.spark");
            directory_namespace = Path.Combine(directory, "!namespace");
            directory_namespace_oneSpark = Path.Combine(directory_namespace, "one.spark");

            Directory.CreateDirectory(directory);
            Directory.CreateDirectory(directory_sub);
            Directory.CreateDirectory(directory_namespace);

            File.WriteAllText(directory_oneSpark, "");
            File.WriteAllText(directory_twoSpark, "");
            File.WriteAllText(directory_namespaceSpark, "");
            File.WriteAllText(directory_sub_oneSpark, "");
            File.WriteAllText(directory_namespace_oneSpark, "");
        }

        [TearDown]
        public void DestroyDirectory()
        {
            Directory.Delete(directory, true);
        }

        [Test]
        public void ReadsDirectories()
        {
            var transformer = MockRepository.GenerateMock<ITemplateTransformer>();
            var bulkTransformer = new BulkTransformer(transformer);
            var namespaces = new Namespace[0];

            bulkTransformer.TransformDirectory(directory, namespaces);

            transformer.AssertWasCalled(x => x.Transform(directory_oneSpark, namespaces));
            transformer.AssertWasCalled(x => x.Transform(directory_twoSpark, namespaces));
            transformer.AssertWasCalled(x => x.Transform(directory_namespaceSpark, namespaces));
        }

        [Test]
        public void ReadsSubDirectories()
        {
            var transformer = MockRepository.GenerateMock<ITemplateTransformer>();
            var bulkTransformer = new BulkTransformer(transformer);
            var namespaces = new Namespace[0];

            bulkTransformer.TransformDirectory(directory, namespaces);

            transformer.AssertWasCalled(x => x.Transform(directory_sub_oneSpark, namespaces));
        }

        [Test]
        public void ReadsSubNamespaceDirectories()
        {
            var transformer = MockRepository.GenerateMock<ITemplateTransformer>();
            var bulkTransformer = new BulkTransformer(transformer);
            var namespaces = new Namespace[0];

            bulkTransformer.TransformDirectory(directory, namespaces);

            transformer.AssertWasCalled(x => x.Transform(directory_namespace_oneSpark, namespaces));
        }
    }
}