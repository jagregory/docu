using System.Reflection;
using DrDoc.Model;
using DrDoc.Parsing;
using Example;
using NUnit.Framework;
using Rhino.Mocks;
using CList = Rhino.Mocks.Constraints.List;
using CIs = Rhino.Mocks.Constraints.Is;

namespace DrDoc.Tests
{
    [TestFixture]
    public class DocParserTests : BaseFixture
    {
        private string[] Xml = new[] {@"<?xml version=""1.0""?>
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
</doc>"};

        [Test]
        public void TypesFromAssembliesPassedToAssociator()
        {
            var associator = MockRepository.GenerateMock<IAssociator>();
            var transformer = MockRepository.GenerateStub<IAssociationTransformer>();
            var documentableMembers = MockRepository.GenerateStub<IDocumentableMemberFinder>();
            var parser = new DocParser(associator, transformer, documentableMembers);
            var assemblies = new[] {typeof(First).Assembly, typeof(DocParserTests).Assembly};
            var members = DocMembers(typeof(First), typeof(Second));

            documentableMembers.Stub(x => x.GetMembersForDocumenting(null))
                .IgnoreArguments()
                .Return(members);

            parser.Parse(assemblies, new[] {""});

            associator.AssertWasCalled(
                x => x.AssociateMembersWithXml(null, null),
                x => x.IgnoreArguments()
                      .Constraints(
                        CIs.Equal(members),
                        CIs.Anything()));
        }

        [Test]
        public void XmlNodesFromStringPassedToAssociator()
        {
            var associator = MockRepository.GenerateMock<IAssociator>();
            var transformer = MockRepository.GenerateStub<IAssociationTransformer>();
            var documentableMembers = MockRepository.GenerateStub<IDocumentableMemberFinder>();
            var parser = new DocParser(associator, transformer, documentableMembers);
            var assemblies = new Assembly[0];

            parser.Parse(assemblies, Xml);

            associator.AssertWasCalled(
                x => x.AssociateMembersWithXml(null, null),
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
            var documentableMembers = MockRepository.GenerateStub<IDocumentableMemberFinder>();
            var parser = new DocParser(associator, transformer, documentableMembers);
            var assemblies = new Assembly[0];
            var associations = new IDocumentationMember[0];

            associator.Stub(x => x.AssociateMembersWithXml(null, null))
                .IgnoreArguments()
                .Return(associations);

            parser.Parse(assemblies, new[] {""});

            transformer.AssertWasCalled(x => x.Transform(associations));
        }
    }
}