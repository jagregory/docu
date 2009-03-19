using System.Collections.Generic;
using Docu.Documentation;
using Docu.Generation;
using Docu.Documentation;
using Docu.Generation;
using Docu.IO;
using NUnit.Framework;
using Rhino.Mocks;
using Is=Rhino.Mocks.Constraints.Is;

namespace Docu.Tests.DocumentationGeneratorTests
{
    [TestFixture]
    public class Generate : BaseDocumentationGeneratorFixture
    {
        [Test]
        public void should_pass_document_model_to_writer()
        {
            var writer = MockRepository.GenerateMock<IBulkPageWriter>();
            var generator = new DocumentationGenerator(StubAssemblyLoader, StubXmlLoader, StubParser, writer, StubResourceManager);
            var documentModel = new List<Namespace>();

            StubParser.Stub(x => x.CreateDocumentModel(null, null))
                .IgnoreArguments()
                .Return(documentModel);

            generator.SetAssemblies(new []{"unimportant_file_path"});
            generator.Generate();

            writer.AssertWasCalled(x => x.CreatePagesFromDirectory(null, null, null),
                                   x => x.Constraints(Is.Anything(), Is.Anything(), Is.Equal(documentModel)));
        }

        [Test]
        public void should_move_untransformable_resources_from_template_dir()
        {
            var resourceManager = MockRepository.GenerateMock<IUntransformableResourceManager>();
            var generator = new DocumentationGenerator(StubAssemblyLoader, StubXmlLoader, StubParser, StubWriter, resourceManager);

            generator.SetAssemblies(new[] { "unimportant_file_path" });
            generator.SetTemplatePath("template-dir");
            generator.Generate();

            resourceManager.AssertWasCalled(x => x.MoveResources(null, null),
                                            x => x.Constraints(Is.Equal("template-dir"), Is.Anything()));
        }

        [Test]
        public void should_move_untransformable_resources_to_output_dir()
        {
            var resourceManager = MockRepository.GenerateMock<IUntransformableResourceManager>();
            var generator = new DocumentationGenerator(StubAssemblyLoader, StubXmlLoader, StubParser, StubWriter, resourceManager);

            generator.SetAssemblies(new[] { "unimportant_file_path" });
            generator.SetOutputPath("output-dir");
            generator.Generate();

            resourceManager.AssertWasCalled(x => x.MoveResources(null, null),
                                            x => x.Constraints(Is.Anything(), Is.Equal("output-dir")));
        }

        [Test]
        public void should_not_error_if_it_can_not_find_assemblies_to_doc()
        {
            var resourceManager = MockRepository.GenerateMock<IUntransformableResourceManager>();
            var generator = new DocumentationGenerator(StubAssemblyLoader, StubXmlLoader, StubParser, StubWriter, resourceManager);

            generator.SetOutputPath("output-dir");
            generator.Generate();

            StubWriter.AssertWasNotCalled(x => x.CreatePagesFromDirectory(null, null, null), x => x.Constraints(Is.Anything(), Is.Anything(), Is.Anything()));
        }
    }
}