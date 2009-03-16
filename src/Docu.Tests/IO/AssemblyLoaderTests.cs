using Docu.IO;
using Docu.IO;
using NUnit.Framework;

namespace Docu.Tests.IO
{
    [TestFixture]
    public class AssemblyLoaderTests
    {
        [Test]
        // hard to test this without shipping an unloaded assembly with the tests
        public void should_load_assembly_by_name()
        {
            var assembly = typeof(DocumentationGenerator).Assembly;

            new AssemblyLoader()
                .LoadFrom(assembly.Location)
                .ShouldEqual(assembly);
        }
    }
}
