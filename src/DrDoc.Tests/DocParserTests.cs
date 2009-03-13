using System.Reflection;
using DrDoc.Associations;
using DrDoc.Parsing;
using Example;
using NUnit.Framework;
using Rhino.Mocks;
using CList = Rhino.Mocks.Constraints.List;
using CIs = Rhino.Mocks.Constraints.Is;

namespace DrDoc.Tests
{
    [TestFixture]
    public class DocParserTests
    {
        private string Xml = @"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>DrDoc</name>
    </assembly>
    <members>
        <member name=""T:Example.First"">
            <summary>A summary</summary>
        </member>
        <member name=""T:Example.Second"" />
        <member name=""M:Example.Second.SecondMethod"" />
    </members>
</doc>";

        [Test]
        public void TypesFromAssembliesPassedToAssociator()
        {
            var associator = MockRepository.GenerateMock<IAssociator>();
            var transformer = MockRepository.GenerateStub<IAssociationTransformer>();
            var parser = new DocParser(associator, transformer);
            var assemblies = new[] {typeof(First).Assembly, typeof(DocParserTests).Assembly};

            parser.Parse(assemblies, "");

            associator.AssertWasCalled(
                x => x.Examine(null, null),
                x => x.IgnoreArguments()
                      .Constraints(
                        CList.ContainsAll(new []{ typeof(First), typeof(DocParserTests)}),
                        CIs.Anything()));
        }

        [Test]
        public void XmlNodesFromStringPassedToAssociator()
        {
            var associator = MockRepository.GenerateMock<IAssociator>();
            var transformer = MockRepository.GenerateStub<IAssociationTransformer>();
            var parser = new DocParser(associator, transformer);
            var assemblies = new Assembly[0];

            parser.Parse(assemblies, Xml);

            associator.AssertWasCalled(
                x => x.Examine(null, null),
                x => x.IgnoreArguments()
                      .Constraints(
                        CIs.Anything(),
                        CList.Count(CIs.GreaterThan(0))));
        }

        [Test]
        public void ShouldPassAssocationsToTransformer()
        {
            var associator = MockRepository.GenerateStub<IAssociator>();
            var transformer = MockRepository.GenerateMock<IAssociationTransformer>();
            var parser = new DocParser(associator, transformer);
            var assemblies = new Assembly[0];
            var associations = new Association[0];

            associator.Stub(x => x.Examine(null, null))
                .IgnoreArguments()
                .Return(associations);

            parser.Parse(assemblies, "");

            transformer.AssertWasCalled(x => x.Transform(associations));
        }
    }
}