using System.Collections.Generic;
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
              new TypeAssociation("T:Example.First", @"<member name=""T:Example.First"" />".ToNode(), typeof(First)),  
              new TypeAssociation("T:Example.Deep.DeepFirst", @"<member name=""T:Example.Deep.DeepFirst"" />".ToNode(), typeof(DeepFirst))
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
              new TypeAssociation("T:Example.First", @"<member name=""T:Example.First"" />".ToNode(), typeof(First)),  
              new TypeAssociation("T:Example.Second", @"<member name=""T:Example.Second"" />".ToNode(), typeof(Second)),  
              new TypeAssociation("T:Example.Deep.DeepFirst", @"<member name=""T:Example.Deep.DeepFirst"" />".ToNode(), typeof(DeepFirst))
            };
            var namespaces = transformer.Transform(associations);

            namespaces[0].Types
                .ShouldContain(x => x.Name == "First")
                .ShouldContain(x => x.Name == "Second");
            namespaces[1].Types
                .ShouldContain(x => x.Name == "DeepFirst");
        }

        [Test]
        public void ShouldHavePrettyNamesForGenericTypes()
        {
            var associations = new[]
            {
              new TypeAssociation("T:Example.GenericDefinition`1", @"<member name=""T:Example.GenericDefinition`1"" />".ToNode(), typeof(GenericDefinition<>)),  
            };
            var namespaces = transformer.Transform(associations);

            namespaces[0].Types
                .ShouldContain(x => x.PrettyName == "GenericDefinition<T>");
        }

        [Test]
        public void ShouldHaveSummaryForType()
        {
            var associations = new[]
            {
              new TypeAssociation("T:Example.First", @"<member name=""T:Example.First""><summary>First summary</summary></member>".ToNode(), typeof(First)),  
              new TypeAssociation("T:Example.Second", @"<member name=""T:Example.Second""><summary>Second summary</summary></member>".ToNode(), typeof(Second)),  
            };
            var namespaces = transformer.Transform(associations);

            namespaces[0].Types[0].Summary.CountShouldEqual(1);
            ((DocTextBlock)namespaces[0].Types[0].Summary[0]).Text.ShouldEqual("First summary");
            namespaces[0].Types[1].Summary.CountShouldEqual(1);
            ((DocTextBlock)namespaces[0].Types[1].Summary[0]).Text.ShouldEqual("Second summary");
        }

        [Test]
        public void ShouldntHaveAnyUnresolvedReferencesLeftIfAllValid()
        {
            var associations = new[]
            {
              new TypeAssociation("T:Example.First", @"<member name=""T:Example.First"" />".ToNode(), typeof(First)),  
              new TypeAssociation("T:Example.Second", @"<member name=""T:Example.Second""><summary><see cref=""T:Example.First"" /></summary></member>".ToNode(), typeof(Second)),  
            };
            var namespaces = transformer.Transform(associations);

            ((DocReferenceBlock)namespaces[0].Types[1].Summary[0]).Reference.ShouldNotBeOfType<UnresolvedReference>();
            ((DocReferenceBlock)namespaces[0].Types[1].Summary[0]).Reference.ShouldNotBeNull();
        }

        [Test]
        public void UnresolvedReferencesBecomeExternalReferencesIfStillExist()
        {
            var associations = new[]
            {
              new TypeAssociation("T:Example.Second", @"<member name=""T:Example.Second""><summary><see cref=""T:Example.First"" /></summary></member>".ToNode(), typeof(Second)),  
            };
            var namespaces = transformer.Transform(associations);

            ((DocReferenceBlock)namespaces[0].Types[0].Summary[0]).Reference.ShouldBeOfType<ExternalReference>();
            ((DocReferenceBlock)namespaces[0].Types[0].Summary[0]).Reference.Name.ShouldEqual("First");
            ((ExternalReference)((DocReferenceBlock)namespaces[0].Types[0].Summary[0]).Reference).FullName.ShouldEqual("Example.First");
        }

        [Test]
        public void ShouldPassSummaryToContentParser()
        {
            var contentParser = MockRepository.GenerateMock<ICommentContentParser>();
            var associations = new[] { new TypeAssociation("T:Example.First", @"<member name=""T:Example.First""><summary>First summary</summary></member>".ToNode(), typeof(First)) };

            contentParser.Stub(x => x.Parse(null))
                .IgnoreArguments()
                .Return(new List<DocBlock>());
            
            new AssociationTransformer(contentParser).Transform(associations);

            contentParser.AssertWasCalled(x => x.Parse(associations[0].Xml.ChildNodes[0]));
        }

        [Test]
        public void ShouldForceTypeIfOnlyMethodDefined()
        {
            var associations = new Association[]
            {
              new MethodAssociation("M:Example.Second.SecondMethod", @"<member name=""M:Example.Second.SecondMethod"" />".ToNode(), typeof(Second).GetMethod("SecondMethod")),
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
              new TypeAssociation("T:Example.Second", @"<member name=""T:Example.Second"" />".ToNode(), typeof(Second)),  
              new MethodAssociation("M:Example.Second.SecondMethod", @"<member name=""M:Example.Second.SecondMethod"" />".ToNode(), typeof(Second).GetMethod("SecondMethod")),
              new MethodAssociation("M:Example.Second.SecondMethod2(System.String,System.Int32)", @"<member name=""M:Example.Second.SecondMethod2(System.String,System.Int32)"" />".ToNode(), typeof(Second).GetMethod("SecondMethod2"))
            };
            var namespaces = transformer.Transform(associations);

            namespaces[0].Types[0].Methods
                .ShouldContain(x => x.Name == "SecondMethod")
                .ShouldContain(x => x.Name == "SecondMethod");
        }

        [Test]
        public void ShouldHaveSummaryForMethods()
        {
            var associations = new[]
            {
              new MethodAssociation("M:Example.Second.SecondMethod", @"<member name=""M:Example.Second.SecondMethod""><summary>Second method</summary></member>".ToNode(), typeof(Second).GetMethod("SecondMethod")),
              new MethodAssociation("M:Example.Second.SecondMethod2(System.String,System.Int32)", @"<member name=""M:Example.Second.SecondMethod2(System.String,System.Int32)""><summary>Second method 2</summary></member>".ToNode(), typeof(Second).GetMethod("SecondMethod2"))
            };
            var namespaces = transformer.Transform(associations);

            namespaces[0].Types[0].Methods[0].Summary.CountShouldEqual(1);
            ((DocTextBlock)namespaces[0].Types[0].Methods[0].Summary[0]).Text.ShouldEqual("Second method");
            namespaces[0].Types[0].Methods[1].Summary.CountShouldEqual(1);
            ((DocTextBlock)namespaces[0].Types[0].Methods[1].Summary[0]).Text.ShouldEqual("Second method 2");
        }

        [Test]
        public void ShouldPassMethodSummaryToContentParser()
        {
            var contentParser = MockRepository.GenerateMock<ICommentContentParser>();
            var associations = new[] { new MethodAssociation("M:Example.Second.SecondMethod", @"<member name=""M:Example.Second.SecondMethod""><summary>First summary</summary></member>".ToNode(), typeof(Second).GetMethod("SecondMethod")) };

            contentParser.Stub(x => x.Parse(null))
                .IgnoreArguments()
                .Return(new List<DocBlock>());

            new AssociationTransformer(contentParser).Transform(associations);

            contentParser.AssertWasCalled(x => x.Parse(associations[0].Xml.ChildNodes[0]));
        }

        [Test]
        public void ShouldHavePropertiesInTypes()
        {
            var associations = new Association[]
            {
              new TypeAssociation("T:Example.Second", @"<member name=""T:Example.Second"" />".ToNode(), typeof(Second)),  
              new PropertyAssociation("M:Example.Second.SecondProperty", @"<member name=""M:Example.Second.SecondProperty"" />".ToNode(), typeof(Second).GetProperty("SecondProperty")),
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
              new TypeAssociation("T:Example.Second", @"<member name=""T:Example.Second"" />".ToNode(), typeof(Second)),  
              new MethodAssociation("M:Example.Second.SecondMethod2(System.String,System.Int32)", @"<member name=""M:Example.Second.SecondMethod2(System.String,System.Int32)"" />".ToNode(), typeof(Second).GetMethod("SecondMethod2"))
            };
            var namespaces = transformer.Transform(associations);

            namespaces[0].Types[0].Methods[0].Parameters
                .ShouldContain(x => x.Name == "one" && x.Type == "System.String")
                .ShouldContain(x => x.Name == "two" && x.Type == "System.Int32");
        }

        [Test]
        public void ShouldHaveSummaryForMethodParameter()
        {
            var associations = new[]
            {
              new MethodAssociation("M:Example.Second.SecondMethod2(System.String,System.Int32)", @"
                <member name=""M:Example.Second.SecondMethod2(System.String,System.Int32)"">
                  <param name=""one"">First parameter</param>
                  <param name=""two"">Second parameter</param>
                </member>".ToNode(), typeof(Second).GetMethod("SecondMethod2"))
            };
            var namespaces = transformer.Transform(associations);

            namespaces[0].Types[0].Methods[0].Parameters[0].Summary.CountShouldEqual(1);
            ((DocTextBlock)namespaces[0].Types[0].Methods[0].Parameters[0].Summary[0]).Text.ShouldEqual("First parameter");
            namespaces[0].Types[0].Methods[0].Parameters[1].Summary.CountShouldEqual(1);
            ((DocTextBlock)namespaces[0].Types[0].Methods[0].Parameters[1].Summary[0]).Text.ShouldEqual("Second parameter");
        }

        [Test]
        public void ShouldPassMethodParameterSummaryToContentParser()
        {
            var contentParser = MockRepository.GenerateMock<ICommentContentParser>();
            var associations = new[]
            {
                new MethodAssociation("M:Example.Second.SecondMethod2(System.String,System.Int32)", @"
                <member name=""M:Example.Second.SecondMethod2(System.String,System.Int32)"">
                  <param name=""one"">First parameter</param>
                  <param name=""two"">Second parameter</param>
                </member>".ToNode(), typeof(Second).GetMethod("SecondMethod2"))
            };

            contentParser.Stub(x => x.Parse(null))
                .IgnoreArguments()
                .Return(new List<DocBlock>());

            new AssociationTransformer(contentParser).Transform(associations);

            contentParser.AssertWasCalled(x => x.Parse(associations[0].Xml.ChildNodes[0]));
            contentParser.AssertWasCalled(x => x.Parse(associations[0].Xml.ChildNodes[1]));
        }
    }
}