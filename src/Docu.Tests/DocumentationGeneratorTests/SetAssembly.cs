using Docu.IO;
using Docu.Parsing;
using Docu.Generation;
using Docu.IO;
using Docu.Parsing;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using TestFixture = NUnit.Framework.TestFixtureAttribute;
using Test = NUnit.Framework.TestAttribute;
using SetUp = NUnit.Framework.SetUpAttribute;

namespace Docu.Tests.DocumentationGeneratorTests
{
    [TestFixture]
    public class SetAssembly : BaseDocumentationGeneratorFixture
    {
        [Test]
        public void should_load_assemblies_if_strings_used()
        {
            var assemblyLoader = MockRepository.GenerateMock<IAssemblyLoader>();
            var generator = new DocumentationGenerator(assemblyLoader, StubXmlLoader, StubParser, StubWriter, StubResourceManager);

            generator.SetAssemblies(new[] { "assembly.dll", "assembly2.dll" });

            assemblyLoader.AssertWasCalled(x => x.LoadFrom("assembly.dll"));
            assemblyLoader.AssertWasCalled(x => x.LoadFrom("assembly2.dll"));
        }

        [Test]
        public void generate_should_pass_assemblies_to_parser_when_set_by_name()
        {
            var parser = MockRepository.GenerateMock<IAssemblyXmlParser>();
            var generator = new DocumentationGenerator(StubAssemblyLoader, StubXmlLoader, parser, StubWriter, StubResourceManager);

            StubAssemblyLoader.Stub(x => x.LoadFrom(null))
                .IgnoreArguments()
                .Return(typeof(IAssemblyLoader).Assembly);

            generator.SetAssemblies(new[] { "assembly.dll" });
            generator.Generate();

            parser.AssertWasCalled(x => x.CreateDocumentModel(null, null),
                x => x.Constraints(List.ContainsAll(new[] { typeof(IAssemblyLoader).Assembly }), Is.Anything()));
        }

        [Test]
        public void generate_should_pass_assemblies_to_parser_when_set_directly()
        {
            var parser = MockRepository.GenerateMock<IAssemblyXmlParser>();
            var generator = new DocumentationGenerator(StubAssemblyLoader, StubXmlLoader, parser, StubWriter, StubResourceManager);

            generator.SetAssemblies(new[] { typeof(IAssemblyLoader).Assembly });
            generator.Generate();

            parser.AssertWasCalled(x => x.CreateDocumentModel(null, null),
                x => x.Constraints(List.ContainsAll(new[] { typeof(IAssemblyLoader).Assembly }), Is.Anything()));
        }

        [Test]
        public void set_assemblies_should_fire_event_if_bad_file_found()
        {
            var parser = MockRepository.GenerateMock<IAssemblyXmlParser>();
            var badFileFound = false;

            var generator = new DocumentationGenerator(new AssemblyLoader(), StubXmlLoader, parser, StubWriter, StubResourceManager);
            generator.BadFileEvent += (s, e) => badFileFound = true;

            generator.SetAssemblies(new[] { "docu.pdb" });

            badFileFound.ShouldBeTrue();
        }
    }
}