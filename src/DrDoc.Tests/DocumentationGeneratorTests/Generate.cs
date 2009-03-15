using System.Collections.Generic;
using DrDoc.Documentation;
using DrDoc.Generation;
using NUnit.Framework;
using Rhino.Mocks;
using Is=Rhino.Mocks.Constraints.Is;

namespace DrDoc.Tests.DocumentationGeneratorTests
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

            generator.Generate();

            writer.AssertWasCalled(x => x.CreatePagesFromDirectory(null, null, null),
                                   x => x.Constraints(Is.Anything(), Is.Anything(), Is.Equal(documentModel)));
        }

        [Test]
        public void should_move_untransformable_resources_from_template_dir()
        {
            var resourceManager = MockRepository.GenerateMock<IUntransformableResourceManager>();
            var generator = new DocumentationGenerator(StubAssemblyLoader, StubXmlLoader, StubParser, StubWriter, resourceManager);

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

            generator.SetOutputPath("output-dir");
            generator.Generate();

            resourceManager.AssertWasCalled(x => x.MoveResources(null, null),
                                            x => x.Constraints(Is.Anything(), Is.Equal("output-dir")));
        }
    }
}