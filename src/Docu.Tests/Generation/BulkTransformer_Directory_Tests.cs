using System.IO;
using Docu.Documentation;
using Docu.Generation;
using NUnit.Framework;
using Rhino.Mocks;

namespace Docu.Tests.Generation
{
    [TestFixture]
    public class BulkTransformer_Directory_Tests
    {
        private string directory;
        private string directory_sub;
        private string directory_namespace;
        private string directory_oneSpark;
        private string directory_twoSpark;
        private string directory_underscoreSpark;
        private string directory_namespaceSpark;
        private string directory_sub_oneSpark;
        private string directory_namespace_oneSpark;

        [SetUp]
        public void CreateDirectory()
        {
            directory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            directory_oneSpark = Path.Combine(directory, "one.spark");
            directory_twoSpark = Path.Combine(directory, "two.spark");
            directory_underscoreSpark = Path.Combine(directory, "_underscore.spark");
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
            File.WriteAllText(directory_underscoreSpark, "");
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
            var transformer = MockRepository.GenerateMock<IPageWriter>();
            var bulkTransformer = new BulkPageWriter(transformer);
            var namespaces = new Namespace[0];

            bulkTransformer.CreatePagesFromDirectory(directory, "output", namespaces);

            transformer.AssertWasCalled(x => x.CreatePages(directory_oneSpark, "output", namespaces));
            transformer.AssertWasCalled(x => x.CreatePages(directory_twoSpark, "output", namespaces));
            transformer.AssertWasCalled(x => x.CreatePages(directory_namespaceSpark, "output", namespaces));
        }

        [Test]
        public void ReadsSubDirectories()
        {
            var transformer = MockRepository.GenerateMock<IPageWriter>();
            var bulkTransformer = new BulkPageWriter(transformer);
            var namespaces = new Namespace[0];

            bulkTransformer.CreatePagesFromDirectory(directory, "output", namespaces);

            transformer.AssertWasCalled(x => x.CreatePages(directory_sub_oneSpark, "output", namespaces));
        }

        [Test]
        public void ReadsSubNamespaceDirectories()
        {
            var transformer = MockRepository.GenerateMock<IPageWriter>();
            var bulkTransformer = new BulkPageWriter(transformer);
            var namespaces = new Namespace[0];

            bulkTransformer.CreatePagesFromDirectory(directory, "output", namespaces);

            transformer.AssertWasCalled(x => x.CreatePages(directory_namespace_oneSpark, "output", namespaces));
        }

        [Test]
        public void CanWriteToOutputDirectory()
        {
            var transformer = MockRepository.GenerateMock<IPageWriter>();
            var bulkTransformer = new BulkPageWriter(transformer);
            var namespaces = new Namespace[0];

            bulkTransformer.CreatePagesFromDirectory(directory_sub, "output", namespaces);

            transformer.AssertWasCalled(x => x.CreatePages(directory_sub_oneSpark, "output", namespaces));
        }

        [Test]
        public void should_pass_template_path_to_writer()
        {
            var transformer = MockRepository.GenerateMock<IPageWriter>();
            var bulkTransformer = new BulkPageWriter(transformer);
            var namespaces = new Namespace[0];

            bulkTransformer.CreatePagesFromDirectory(directory_sub, "output", namespaces);

            transformer.AssertWasCalled(x => x.SetTemplatePath(directory_sub));
        }

        [Test]
        public void shouldnt_parse_underscore_prefixed_spark_files()
        {
            var transformer = MockRepository.GenerateMock<IPageWriter>();
            var bulkTransformer = new BulkPageWriter(transformer);
            var namespaces = new Namespace[0];

            bulkTransformer.CreatePagesFromDirectory(directory, "output", namespaces);

            transformer.AssertWasNotCalled(x => x.CreatePages(directory_underscoreSpark, "output", namespaces));
        }
    }
}