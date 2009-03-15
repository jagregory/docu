using System.Reflection;
using DrDoc.Documentation;
using DrDoc.Parsing.Model;
using DrDoc.Parsing;
using Example;
using NUnit.Framework;
using Rhino.Mocks;
using CList = Rhino.Mocks.Constraints.List;
using CIs = Rhino.Mocks.Constraints.Is;

namespace DrDoc.Tests.Parsing
{
    [TestFixture]
    public class AssemblyXmlParserTests : BaseFixture
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
            var matcher = MockRepository.GenerateMock<IDocumentationXmlMatcher>();
            var model = MockRepository.GenerateStub<IDocumentModel>();
            var documentableMembers = MockRepository.GenerateStub<IDocumentableMemberFinder>();
            var parser = new AssemblyXmlParser(matcher, model, documentableMembers);
            var assemblies = new[] {typeof(First).Assembly, typeof(AssemblyXmlParserTests).Assembly};
            var members = DocMembers(typeof(First), typeof(Second));

            documentableMembers.Stub(x => x.GetMembersForDocumenting(null))
                .IgnoreArguments()
                .Return(members);

            parser.CreateDocumentModel(assemblies, new[] {""});

            matcher.AssertWasCalled(
                x => x.DocumentMembers(null, null),
                x => x.IgnoreArguments()
                         .Constraints(
                         CIs.Equal(members),
                         CIs.Anything()));
        }

        [Test]
        public void XmlNodesFromStringPassedToAssociator()
        {
            var matcher = MockRepository.GenerateMock<IDocumentationXmlMatcher>();
            var model = MockRepository.GenerateStub<IDocumentModel>();
            var documentableMembers = MockRepository.GenerateStub<IDocumentableMemberFinder>();
            var parser = new AssemblyXmlParser(matcher, model, documentableMembers);
            var assemblies = new Assembly[0];

            parser.CreateDocumentModel(assemblies, Xml);

            matcher.AssertWasCalled(
                x => x.DocumentMembers(null, null),
                x => x.IgnoreArguments()
                         .Constraints(
                         CIs.Anything(),
                         CList.Count(CIs.GreaterThan(0))));
        }

        [Test]
        public void ShouldPassAssocationsToTransformer()
        {
            var matcher = MockRepository.GenerateStub<IDocumentationXmlMatcher>();
            var model = MockRepository.GenerateMock<IDocumentModel>();
            var documentableMembers = MockRepository.GenerateStub<IDocumentableMemberFinder>();
            var parser = new AssemblyXmlParser(matcher, model, documentableMembers);
            var assemblies = new Assembly[0];
            var associations = new IDocumentationMember[0];

            matcher.Stub(x => x.DocumentMembers(null, null))
                .IgnoreArguments()
                .Return(associations);

            parser.CreateDocumentModel(assemblies, new[] {""});

            model.AssertWasCalled(x => x.Create(associations));
        }
    }
}