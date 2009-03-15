using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using DrDoc.Model;
using DrDoc.Parsing;
using Example;
using Example.Deep;
using NUnit.Framework;
using Rhino.Mocks;

namespace DrDoc.Tests
{
    [TestFixture]
    public class AssociationTransformerTests : BaseFixture
    {
        private AssociationTransformer transformer;

        [SetUp]
        public void CreateTransformer()
        {
            transformer = new AssociationTransformer(new CommentContentParser());
        }

        private DocumentedType TypeAssociation<T>(string xml)
        {
            return new DocumentedType(Identifier.FromType(typeof(T)), xml.ToNode(), typeof(T));
        }

        private DocumentedType TypeAssociation(Type type, string xml)
        {
            return new DocumentedType(Identifier.FromType(type), xml.ToNode(), type);
        }

        private DocumentedMethod MethodAssociation<T>(string xml, Expression<Action<T>> methodAction)
        {
            var method = ((MethodCallExpression)methodAction.Body).Method;

            return new DocumentedMethod(Identifier.FromMethod(method, typeof(T)), xml.ToNode(), method, typeof(T));
        }

        private DocumentedProperty PropertyAssociation<T>(string xml, Expression<Func<T, object>> propertyAction)
        {
            var property = ((MemberExpression)propertyAction.Body).Member as PropertyInfo;

            return new DocumentedProperty(Identifier.FromProperty(property, typeof(T)), xml.ToNode(), property);
        }

        [Test]
        public void ShouldBuildNamespaces()
        {
            var associations = new[]
            {
              TypeAssociation<First>(@"<member name=""T:Example.First"" />"),  
              TypeAssociation<DeepFirst>(@"<member name=""T:Example.Deep.DeepFirst"" />"),
            };
            var namespaces = transformer.Transform(associations);

            namespaces.ShouldContain(x => x.Name == Identifier.FromNamespace("Example"));
            namespaces.ShouldContain(x => x.Name == Identifier.FromNamespace("Example.Deep"));
        }

        [Test]
        public void ShouldHaveTypesInNamespaces()
        {
            var associations = new[]
            {
              TypeAssociation<First>(@"<member name=""T:Example.First"" />"),  
              TypeAssociation<Second>(@"<member name=""T:Example.Second"" />"),  
              TypeAssociation<DeepFirst>(@"<member name=""T:Example.Deep.DeepFirst"" />")
            };
            var namespaces = transformer.Transform(associations);

            namespaces[0].Types
                .ShouldContain(x => x.Name == Identifier.FromType(typeof(First)))
                .ShouldContain(x => x.Name == Identifier.FromType(typeof(Second)));
            namespaces[1].Types
                .ShouldContain(x => x.Name == Identifier.FromType(typeof(DeepFirst)));
        }

        [Test]
        public void ShouldHavePrettyNamesForGenericTypes()
        {
            var associations = new[]
            {
              TypeAssociation(typeof(GenericDefinition<>), @"<member name=""T:Example.GenericDefinition`1"" />"),  
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
              TypeAssociation<First>(@"<member name=""T:Example.First""><summary>First summary</summary></member>"),
              TypeAssociation<Second>(@"<member name=""T:Example.Second""><summary>Second summary</summary></member>"),
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
              TypeAssociation<First>(@"<member name=""T:Example.First"" />"),  
              TypeAssociation<Second>(@"<member name=""T:Example.Second""><summary><see cref=""T:Example.First"" /></summary></member>"),  
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
              TypeAssociation<Second>(@"<member name=""T:Example.Second""><summary><see cref=""T:Example.First"" /></summary></member>"),  
            };
            var namespaces = transformer.Transform(associations);

            ((DocReferenceBlock)namespaces[0].Types[0].Summary[0]).Reference.ShouldBeOfType<ExternalReference>();
            ((DocReferenceBlock)namespaces[0].Types[0].Summary[0]).Reference.Name.ShouldEqual(Identifier.FromType(typeof(First)));
            ((ExternalReference)((DocReferenceBlock)namespaces[0].Types[0].Summary[0]).Reference).FullName.ShouldEqual("Example.First");
        }

        [Test]
        public void ShouldPassSummaryToContentParser()
        {
            var contentParser = MockRepository.GenerateMock<ICommentContentParser>();
            var associations = new[] { TypeAssociation<First>(@"<member name=""T:Example.First""><summary>First summary</summary></member>") };

            contentParser.Stub(x => x.Parse(null))
                .IgnoreArguments()
                .Return(new List<DocBlock>());
            
            new AssociationTransformer(contentParser).Transform(associations);

            contentParser.AssertWasCalled(x => x.Parse(associations[0].Xml.ChildNodes[0]));
        }

        [Test]
        public void ShouldForceTypeIfOnlyMethodDefined()
        {
            var associations = new IDocumentationMember[]
            {
              MethodAssociation<Second>(@"<member name=""M:Example.Second.SecondMethod"" />", x => x.SecondMethod()),
            };
            var namespaces = transformer.Transform(associations);

            namespaces[0].Name.ShouldEqual(Identifier.FromNamespace("Example"));
            namespaces[0].Types.ShouldContain(x => x.Name == Identifier.FromType(typeof(Second)));
        }

        [Test]
        public void ShouldHaveMethodsInTypes()
        {
            var associations = new IDocumentationMember[]
            {
              TypeAssociation<Second>(@"<member name=""T:Example.Second"" />"),  
              MethodAssociation<Second>(@"<member name=""M:Example.Second.SecondMethod"" />", x => x.SecondMethod()),
              MethodAssociation<Second>(@"<member name=""M:Example.Second.SecondMethod2(System.String,System.Int32)"" />", x => x.SecondMethod2(null, 0))
            };
            var namespaces = transformer.Transform(associations);
            var method = Method<Second>(x => x.SecondMethod());
            var method2 = Method<Second>(x => x.SecondMethod2(null, 0));

            namespaces[0].Types[0].Methods
                .ShouldContain(x => x.Name == Identifier.FromMethod(method, typeof(Second)))
                .ShouldContain(x => x.Name == Identifier.FromMethod(method2, typeof(Second)));
        }

        [Test]
        public void ShouldHaveSummaryForMethods()
        {
            var associations = new[]
            {
              MethodAssociation<Second>(@"<member name=""M:Example.Second.SecondMethod""><summary>Second method</summary></member>", x => x.SecondMethod()),
              MethodAssociation<Second>(@"<member name=""M:Example.Second.SecondMethod2(System.String,System.Int32)""><summary>Second method 2</summary></member>", x => x.SecondMethod2(null, 0))
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
            var associations = new[] { MethodAssociation<Second>(@"<member name=""M:Example.Second.SecondMethod""><summary>First summary</summary></member>", x => x.SecondMethod()) };

            contentParser.Stub(x => x.Parse(null))
                .IgnoreArguments()
                .Return(new List<DocBlock>());

            new AssociationTransformer(contentParser).Transform(associations);

            contentParser.AssertWasCalled(x => x.Parse(associations[0].Xml.ChildNodes[0]));
        }

        [Test]
        public void ShouldHavePropertiesInTypes()
        {
            var associations = new IDocumentationMember[]
            {
              TypeAssociation<Second>(@"<member name=""T:Example.Second"" />"),
              PropertyAssociation<Second>(@"<member name=""M:Example.Second.SecondProperty"" />", x => x.SecondProperty)
            };
            var namespaces = transformer.Transform(associations);

            namespaces[0].Types[0].Properties
                .ShouldContain(x => x.Name == "SecondProperty");
        }

        [Test]
        public void ShouldHaveParametersInMethods()
        {
            var associations = new IDocumentationMember[]
            {
                TypeAssociation<First>(@"<member name=""T:Example.First"" />"),
                MethodAssociation<Second>(@"<member name=""M:Example.Second.SecondMethod2(System.String,Example.First)"" />", x => x.SecondMethod3(null, null))
            };
            var namespaces = transformer.Transform(associations);

            var method = namespaces[0].Types[1].Methods[0];

            method.Parameters.CountShouldEqual(2);
            method.Parameters[0].Name.ShouldEqual("one");
            method.Parameters[0].Reference.ShouldBeOfType<ExternalReference>();
            method.Parameters[1].Name.ShouldEqual("two");
            method.Parameters[1].Reference.ShouldBeOfType<DocType>();
        }

        [Test]
        public void ShouldHaveSummaryForMethodParameter()
        {
            var associations = new[]
            {
              MethodAssociation<Second>(@"
                <member name=""M:Example.Second.SecondMethod2(System.String,System.Int32)"">
                  <param name=""one"">First parameter</param>
                  <param name=""two"">Second parameter</param>
                </member>", x => x.SecondMethod2(null, 0))
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
                MethodAssociation<Second>(@"
                <member name=""M:Example.Second.SecondMethod2(System.String,System.Int32)"">
                  <param name=""one"">First parameter</param>
                  <param name=""two"">Second parameter</param>
                </member>", x => x.SecondMethod2(null, 0))
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