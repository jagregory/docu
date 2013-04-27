using Docu.IO;
using NUnit.Framework;
using Rhino.Mocks;

namespace Docu.Tests.DocumentationGeneratorTests
{
    public class BaseDocumentationGeneratorFixture
    {
        protected IAssemblyLoader StubAssemblyLoader;

        [SetUp]
        public void CreateStubs()
        {
            StubAssemblyLoader = MockRepository.GenerateStub<IAssemblyLoader>();

            StubAssemblyLoader.Stub(x => x.LoadFrom(null))
                .IgnoreArguments()
                .Return(typeof(DocumentationGenerator).Assembly);
        }
    }
}