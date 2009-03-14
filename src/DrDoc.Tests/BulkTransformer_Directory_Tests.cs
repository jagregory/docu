using System.IO;
using DrDoc.Generation;
using NUnit.Framework;
using Rhino.Mocks;

namespace DrDoc.Tests
{
    [TestFixture]
    public class BulkTransformer_Directory_Tests
    {
        private string directoryPath;
        private string onePath;
        private string twoPath;
        private string namespacePath;

        [SetUp]
        public void CreateDirectory()
        {
            directoryPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            onePath = Path.Combine(directoryPath, "one.spark");
            twoPath = Path.Combine(directoryPath, "two.spark");
            namespacePath = Path.Combine(directoryPath, "!namespace.spark");

            Directory.CreateDirectory(directoryPath);

            File.WriteAllText(onePath, "");
            File.WriteAllText(twoPath, "");
            File.WriteAllText(namespacePath, "");
        }

        [TearDown]
        public void DestroyDirectory()
        {
            Directory.Delete(directoryPath, true);
        }

        [Test]
        public void ReadsDirectories()
        {
            var transformer = MockRepository.GenerateMock<ITemplateTransformer>();
            var bulkTransformer = new BulkTransformer(transformer);
            var namespaces = new DocNamespace[0];

            bulkTransformer.TransformDirectory(directoryPath, namespaces);

            transformer.AssertWasCalled(x => x.Transform(onePath, namespaces));
            transformer.AssertWasCalled(x => x.Transform(twoPath, namespaces));
            transformer.AssertWasCalled(x => x.Transform(namespacePath, namespaces));
        }
    }
}