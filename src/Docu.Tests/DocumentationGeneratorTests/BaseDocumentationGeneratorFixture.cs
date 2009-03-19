using Docu.Generation;
using Docu.IO;
using Docu.Parsing;
using Docu.Generation;
using Docu.IO;
using Docu.Parsing;
using NUnit.Framework;
using Rhino.Mocks;

namespace Docu.Tests.DocumentationGeneratorTests
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

            StubAssemblyLoader.Stub(x => x.LoadFrom(null))
                .IgnoreArguments()
                .Return(typeof(DocumentationGenerator).Assembly);
        }
    }
}