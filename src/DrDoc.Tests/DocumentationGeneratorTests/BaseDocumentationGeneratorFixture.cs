using DrDoc.Generation;
using DrDoc.IO;
using DrDoc.Parsing;
using NUnit.Framework;
using Rhino.Mocks;

namespace DrDoc.Tests.DocumentationGeneratorTests
{
    public class BaseDocumentationGeneratorFixture
    {
        protected IBulkPageWriter StubWriter;
        protected IAssemblyXmlParser StubParser;
        protected IUntransformableResourceManager StubResourceManager;
        protected IAssemblyLoader StubAssemblyLoader;
        protected IXmlLoader StubXmlLoader;

        [SetUp]
        public void CreateStubs()
        {
            StubWriter = MockRepository.GenerateStub<IBulkPageWriter>();
            StubParser = MockRepository.GenerateStub<IAssemblyXmlParser>();
            StubResourceManager = MockRepository.GenerateStub<IUntransformableResourceManager>();
            StubAssemblyLoader = MockRepository.GenerateStub<IAssemblyLoader>();
            StubXmlLoader = MockRepository.GenerateStub<IXmlLoader>();
        }
    }
}