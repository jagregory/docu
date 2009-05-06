using Docu.Generation;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using TestFixture = NUnit.Framework.TestFixtureAttribute;
using Test = NUnit.Framework.TestAttribute;

namespace Docu.Tests.DocumentationGeneratorTests
{
    [TestFixture]
    public class SetTemplatePath : BaseDocumentationGeneratorFixture
    {
        [Test]
        public void should_pass_template_path_to_writer_if_set()
        {
            var writer = MockRepository.GenerateMock<IBulkPageWriter>();
            var generator = new DocumentationGenerator(StubAssemblyLoader, StubXmlLoader, StubParser, writer, StubResourceManager, StubEventAggregator);

            generator.SetAssemblies(new[] { "unimportant_file_path" });
            generator.SetTemplatePath("template-path");
            generator.Generate();

            writer.AssertWasCalled(x => x.CreatePagesFromDirectory(null, null, null),
                x => x.Constraints(Is.Equal("template-path"), Is.Anything(), Is.Anything()));
        }

        [Test]
        public void should_pass_default_template_path_to_writer_if_not_set()
        {
            var writer = MockRepository.GenerateMock<IBulkPageWriter>();
            var generator = new DocumentationGenerator(StubAssemblyLoader, StubXmlLoader, StubParser, writer, StubResourceManager, StubEventAggregator);

            generator.SetAssemblies(new[] { "unimportant_file_path" });
            generator.Generate();

            writer.AssertWasCalled(x => x.CreatePagesFromDirectory(null, null, null),
                x => x.Constraints(Text.EndsWith("templates"), Is.Anything(), Is.Anything()));
        }
    }
}