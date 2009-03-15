using DrDoc.Generation;
using DrDoc.IO;
using DrDoc.Parsing;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using TestFixture = NUnit.Framework.TestFixtureAttribute;
using Test = NUnit.Framework.TestAttribute;
using SetUp = NUnit.Framework.SetUpAttribute;

namespace DrDoc.Tests.DocumentationGeneratorTests
{
    [TestFixture]
    public class SetXml : BaseDocumentationGeneratorFixture
    {
        [Test]
        public void should_load_xml_files_if_names_used()
        {
            var xmlLoader = MockRepository.GenerateMock<IXmlLoader>();
            var generator = new DocumentationGenerator(StubAssemblyLoader, xmlLoader, StubParser, StubWriter, StubResourceManager);

            generator.SetXmlFiles(new[] { "assembly.xml", "assembly2.xml" });

            xmlLoader.AssertWasCalled(x => x.LoadFrom("assembly.xml"));
            xmlLoader.AssertWasCalled(x => x.LoadFrom("assembly2.xml"));
        }

        [Test]
        public void generate_should_pass_xmls_to_parser_when_set_by_name()
        {
            var parser = MockRepository.GenerateMock<IAssemblyXmlParser>();
            var generator = new DocumentationGenerator(StubAssemblyLoader, StubXmlLoader, parser, StubWriter, StubResourceManager);

            StubXmlLoader.Stub(x => x.LoadFrom(null))
                .IgnoreArguments()
                .Return("content");

            generator.SetXmlFiles(new[] { "assembly.xml" });
            generator.Generate();

            parser.AssertWasCalled(x => x.CreateDocumentModel(null, null),
                x => x.Constraints(Is.Anything(), List.ContainsAll(new[] { "content" })));
        }

        [Test]
        public void generate_should_pass_assemblies_to_parser_when_set_directly()
        {
            var parser = MockRepository.GenerateMock<IAssemblyXmlParser>();
            var generator = new DocumentationGenerator(StubAssemblyLoader, StubXmlLoader, parser, StubWriter, StubResourceManager);

            generator.SetXmlContent(new[] { "content" });
            generator.Generate();

            parser.AssertWasCalled(x => x.CreateDocumentModel(null, null),
                x => x.Constraints(Is.Anything(), List.ContainsAll(new[] { "content" })));
        }
    }
}