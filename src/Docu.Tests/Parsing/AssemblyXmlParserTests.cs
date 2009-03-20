using System.Reflection;
using Docu.Documentation;
using Docu.Parsing;
using Docu.Parsing.Model;
using Docu.Documentation;
using Docu.Parsing.Model;
using Docu.Parsing;
using Example;
using NUnit.Framework;
using Rhino.Mocks;
using CList = Rhino.Mocks.Constraints.List;
using CIs = Rhino.Mocks.Constraints.Is;

namespace Docu.Tests.Parsing
{
    [TestFixture]
    public class AssemblyXmlParserTests : BaseFixture
    {
        private string[] Xml = new[] {@"<?xml version=""1.0""?>
<doc>
    <assembly>
        <name>Docu</name>
    </assembly>
    <members>
        <member name=""T:Example.First"">
            <summary>A summary</summary>
        </member>
        <member name=""T:Example.Second"" />
        <member name=""M:Example.Second.SecondMethod"" />
    </members>
</doc>"};

        private IDocumentationXmlMatcher StubDocXmlMatcher;
        private IDocumentModel StubDocModel;
        private IDocumentableMemberFinder StubDocMembers;

        [SetUp]
        public void CreateStubs()
        {
            StubDocXmlMatcher = MockRepository.GenerateStub<IDocumentationXmlMatcher>();
            StubDocModel = MockRepository.GenerateStub<IDocumentModel>();
            StubDocMembers = MockRepository.GenerateStub<IDocumentableMemberFinder>();
        }

        [Test]
        public void TypesFromAssembliesPassedToAssociator()
        {
            var matcher = MockRepository.GenerateMock<IDocumentationXmlMatcher>();
            var parser = new AssemblyXmlParser(matcher, StubDocModel, StubDocMembers);
            var assemblies = new[] {typeof(First).Assembly, typeof(AssemblyXmlParserTests).Assembly};
            var members = DocMembers(typeof(First), typeof(Second));

            StubDocMembers.Stub(x => x.GetMembersForDocumenting(null))
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
            var parser = new AssemblyXmlParser(matcher, StubDocModel, StubDocMembers);
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
            var model = MockRepository.GenerateMock<IDocumentModel>();
            var parser = new AssemblyXmlParser(StubDocXmlMatcher, model, StubDocMembers);
            var assemblies = new Assembly[0];
            var associations = new IDocumentationMember[0];

            StubDocXmlMatcher.Stub(x => x.DocumentMembers(null, null))
                .IgnoreArguments()
                .Return(associations);

            parser.CreateDocumentModel(assemblies, new[] {""});

            model.AssertWasCalled(x => x.Create(associations));
        }

        [Test]
        public void ShouldRaiseParserWarningWhenMatcherRaisesWarning()
        {
            var model = MockRepository.GenerateMock<IDocumentModel>();
            var parser = new AssemblyXmlParser(StubDocXmlMatcher, model, StubDocMembers);
            var warningRaised = false;
            var warningType = WarningType.Unknown;

            parser.ParseWarning += (sender, e) =>
            {
                warningRaised = true;
                warningType = e.WarningType;
            };

            model.Raise(x => x.CreationWarning += null, model, new DocumentModelWarningEventArgs("Message!"));

            warningRaised.ShouldBeTrue();
            warningType.ShouldEqual(WarningType.DocumentModel);
        }
    }
}