using DrDoc.Associations;
using DrDoc.Parsing;
using Example;
using Example.Deep;
using NUnit.Framework;
using Rhino.Mocks;

namespace DrDoc.Tests
{
    [TestFixture]
    public class AssociationTransformerTests
    {
        private AssociationTransformer transformer;

        [SetUp]
        public void CreateTransformer()
        {
            transformer = new AssociationTransformer(new CommentContentParser());
        }

        [Test]
        public void ShouldBuildNamespaces()
        {
            var associations = new[]
            {
              new TypeAssociation(@"<member name=""T:Example.First"" />".ToNode(), typeof(First)),  
              new TypeAssociation(@"<member name=""T:Example.Deep.DeepFirst"" />".ToNode(), typeof(DeepFirst))
            };
            var namespaces = transformer.Transform(associations);

            namespaces.ShouldContain(x => x.Name == "Example");
            namespaces.ShouldContain(x => x.Name == "Example.Deep");
        }

        [Test]
        public void ShouldHaveTypesInNamespaces()
        {
            var associations = new[]
            {
              new TypeAssociation(@"<member name=""T:Example.First"" />".ToNode(), typeof(First)),  
              new TypeAssociation(@"<member name=""T:Example.Second"" />".ToNode(), typeof(Second)),  
              new TypeAssociation(@"<member name=""T:Example.Deep.DeepFirst"" />".ToNode(), typeof(DeepFirst))
            };
            var namespaces = transformer.Transform(associations);

            namespaces[0].Types
                .ShouldContain(x => x.Name == "First")
                .ShouldContain(x => x.Name == "Second");
            namespaces[1].Types
                .ShouldContain(x => x.Name == "DeepFirst");
        }

        [Test]
        public void ShouldHaveSummaryForType()
        {
            var associations = new[]
            {
              new TypeAssociation(@"<member name=""T:Example.First""><summary>First summary</summary></member>".ToNode(), typeof(First)),  
              new TypeAssociation(@"<member name=""T:Example.Second""><summary>Second summary</summary></member>".ToNode(), typeof(Second)),  
            };
            var namespaces = transformer.Transform(associations);

            namespaces[0].Types[0].Summary.CountShouldEqual(1);
            ((DocTextBlock)namespaces[0].Types[0].Summary[0]).Text.ShouldEqual("First summary");
            namespaces[0].Types[1].Summary.CountShouldEqual(1);
            ((DocTextBlock)namespaces[0].Types[1].Summary[0]).Text.ShouldEqual("Second summary");
        }

        [Test]
        public void ShouldPassSummaryToContentParser()
        {
            var contentParser = MockRepository.GenerateMock<ICommentContentParser>();
            var associations = new[] { new TypeAssociation(@"<member name=""T:Example.First""><summary>First summary</summary></member>".ToNode(), typeof(First)) };
            
            new AssociationTransformer(contentParser).Transform(associations);

            contentParser.AssertWasCalled(x => x.Parse(associations[0].Xml.ChildNodes[0]));
        }

        [Test]
        public void ShouldForceTypeIfOnlyMethodDefined()
        {
            var associations = new Association[]
            {
              new MethodAssociation(@"<member name=""M:Example.Second.SecondMethod"" />".ToNode(), typeof(Second).GetMethod("SecondMethod")),
            };
            var namespaces = transformer.Transform(associations);

            namespaces[0].Name.ShouldEqual("Example");
            namespaces[0].Types.ShouldContain(x => x.Name == "Second");
        }

        [Test]
        public void ShouldHaveMethodsInTypes()
        {
            var associations = new Association[]
            {
              new TypeAssociation(@"<member name=""T:Example.Second"" />".ToNode(), typeof(Second)),  
              new MethodAssociation(@"<member name=""M:Example.Second.SecondMethod"" />".ToNode(), typeof(Second).GetMethod("SecondMethod")),
              new MethodAssociation(@"<member name=""M:Example.Second.SecondMethod2(System.String,System.Int32)"" />".ToNode(), typeof(Second).GetMethod("SecondMethod2"))
            };
            var namespaces = transformer.Transform(associations);

            namespaces[0].Types[0].Methods
                .ShouldContain(x => x.Name == "SecondMethod")
                .ShouldContain(x => x.Name == "SecondMethod");
        }

        [Test]
        public void ShouldHavePropertiesInTypes()
        {
            var associations = new Association[]
            {
              new TypeAssociation(@"<member name=""T:Example.Second"" />".ToNode(), typeof(Second)),  
              new PropertyAssociation(@"<member name=""M:Example.Second.SecondProperty"" />".ToNode(), typeof(Second).GetProperty("SecondProperty")),
            };
            var namespaces = transformer.Transform(associations);

            namespaces[0].Types[0].Properties
                .ShouldContain(x => x.Name == "SecondProperty");
        }

        [Test]
        public void ShouldHaveParametersInMethods()
        {
            var associations = new Association[]
            {
              new TypeAssociation(@"<member name=""T:Example.Second"" />".ToNode(), typeof(Second)),  
              new MethodAssociation(@"<member name=""M:Example.Second.SecondMethod2(System.String,System.Int32)"" />".ToNode(), typeof(Second).GetMethod("SecondMethod2"))
            };
            var namespaces = transformer.Transform(associations);

            namespaces[0].Types[0].Methods[0].Parameters
                .ShouldContain(x => x.Name == "one" && x.Type == "System.String")
                .ShouldContain(x => x.Name == "two" && x.Type == "System.Int32");
        }
    }
}