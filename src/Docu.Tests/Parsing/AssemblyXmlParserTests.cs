using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Docu.Documentation;
using Docu.Parsing;
using Docu.Parsing.Model;
using Example;
using NUnit.Framework;
using Rhino.Mocks;
using CList = Rhino.Mocks.Constraints.List;
using CIs = Rhino.Mocks.Constraints.Is;
using System.Linq;

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
            var capturedArgsForDocumentMembers = matcher.CaptureArgumentsFor(x => x.DocumentMembers(null, null), onCall => onCall.Return(null));

            parser.CreateDocumentModel(assemblies, new[] { "" });
            
            capturedArgsForDocumentMembers.First<IEnumerable<IDocumentationMember>>().ShouldBeSameAs(members);
        }

        [Test]
        public void XmlNodesFromStringPassedToAssociator()
        {
            var matcher = MockRepository.GenerateMock<IDocumentationXmlMatcher>();
            var parser = new AssemblyXmlParser(matcher, StubDocModel, StubDocMembers);
            var assemblies = new Assembly[0];
            var capturedArgsForDocumentMembers = matcher.CaptureArgumentsFor(x => x.DocumentMembers(null, null), onCall => onCall.Return(null));

            parser.CreateDocumentModel(assemblies, Xml);

            capturedArgsForDocumentMembers.Second<IEnumerable<XmlNode>>().Count().ShouldNotEqual(0);
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
    }
}