using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using DrDoc.Documentation;
using DrDoc.Documentation.Comments;
using DrDoc.Parsing.Model;
using DrDoc.Parsing;
using Example;
using Example.Deep;
using NUnit.Framework;
using Rhino.Mocks;

namespace DrDoc.Tests.Documentation
{
    [TestFixture]
    public class DocumentModelTests : BaseFixture
    {
        private DocumentModel model;

        [SetUp]
        public void CreateTransformer()
        {
            model = new DocumentModel(new CommentContentParser());
        }

        private DocumentedType Type<T>(string xml)
        {
            return new DocumentedType(Identifier.FromType(typeof(T)), xml.ToNode(), typeof(T));
        }

        private DocumentedType Type(Type type, string xml)
        {
            return new DocumentedType(Identifier.FromType(type), xml.ToNode(), type);
        }

        private DocumentedMethod Method<T>(string xml, Expression<Action<T>> methodAction)
        {
            var method = ((MethodCallExpression)methodAction.Body).Method;

            return new DocumentedMethod(Identifier.FromMethod(method, typeof(T)), xml.ToNode(), method, typeof(T));
        }

        private DocumentedProperty Property<T>(string xml, Expression<Func<T, object>> propertyAction)
        {
            var property = ((MemberExpression)propertyAction.Body).Member as PropertyInfo;

            return new DocumentedProperty(Identifier.FromProperty(property, typeof(T)), xml.ToNode(), property, typeof(T));
        }

        [Test]
        public void ShouldBuildNamespaces()
        {
            var members = new[]
            {
                Type<First>(@"<member name=""T:Example.First"" />"),  
                Type<DeepFirst>(@"<member name=""T:Example.Deep.DeepFirst"" />"),
            };
            var namespaces = model.Create(members);

            namespaces.ShouldContain(x => x.IsIdentifiedBy(Identifier.FromNamespace("Example")));
            namespaces.ShouldContain(x => x.IsIdentifiedBy(Identifier.FromNamespace("Example.Deep")));
        }

        [Test]
        public void ShouldHaveTypesInNamespaces()
        {
            var members = new[]
            {
                Type<First>(@"<member name=""T:Example.First"" />"),  
                Type<Second>(@"<member name=""T:Example.Second"" />"),  
                Type<DeepFirst>(@"<member name=""T:Example.Deep.DeepFirst"" />")
            };
            var namespaces = model.Create(members);

            namespaces[0].Types
                .ShouldContain(x => x.IsIdentifiedBy(Identifier.FromType(typeof(First))))
                .ShouldContain(x => x.IsIdentifiedBy(Identifier.FromType(typeof(Second))));
            namespaces[1].Types
                .ShouldContain(x => x.IsIdentifiedBy(Identifier.FromType(typeof(DeepFirst))));
        }

        [Test]
        public void ShouldHaveParentForTypes()
        {
            var members = new[]
            {
                Type<First>(@"<member name=""T:Example.First"" />"),  
            };
            var namespaces = model.Create(members);

            namespaces[0].Types[0].ParentType.ShouldNotBeNull();
            namespaces[0].Types[0].ParentType.PrettyName.ShouldEqual("object");
        }

        [Test]
        public void ShouldHaveParentForTypes_WithDocumentedParent()
        {
            var members = new[]
            {
                Type<FirstChild>(@"<member name=""T:Example.FirstChild"" />"),  
            };
            var namespaces = model.Create(members);

            namespaces[0].Types[0].ParentType.ShouldNotBeNull();
            namespaces[0].Types[0].ParentType.PrettyName.ShouldEqual("First");
        }

        [Test]
        public void ShouldHaveInterfacesForTypes()
        {
            var members = new[]
            {
                Type<ClassWithInterfaces>(@"<member name=""T:Example.ClassWithInterfaces"" />"),  
            };
            var namespaces = model.Create(members);

            namespaces[0].Types[0].Interfaces.CountShouldEqual(2);
            namespaces[0].Types[0].Interfaces[0].PrettyName.ShouldEqual("EmptyInterface");
            namespaces[0].Types[0].Interfaces[1].PrettyName.ShouldEqual("IDisposable");
        }

        [Test]
        public void ShouldntInheritInterfacesForTypes()
        {
            var members = new[]
            {
                Type<ClassWithBaseWithInterfaces>(@"<member name=""T:Example.ClassWithBaseWithInterfaces"" />"),  
            };
            var namespaces = model.Create(members);

            namespaces[0].Types[0].Interfaces.CountShouldEqual(0);
        }

        [Test]
        public void ShouldntShowOnlyDirectInterfacesForTypes()
        {
            var members = new[]
            {
                Type<ClassWithBaseWithInterfacesAndDirect>(@"<member name=""T:Example.ClassWithBaseWithInterfacesAndDirect"" />"),  
            };
            var namespaces = model.Create(members);

            namespaces[0].Types[0].Interfaces.CountShouldEqual(1);
            namespaces[0].Types[0].Interfaces[0].PrettyName.ShouldEqual("IExample");
        }

        [Test]
        public void ShouldHavePrettyNamesForGenericTypes()
        {
            var members = new[] { Type(typeof(GenericDefinition<>), @"<member name=""T:Example.GenericDefinition`1"" />") };
            var namespaces = model.Create(members);

            namespaces[0].Types
                .ShouldContain(x => x.PrettyName == "GenericDefinition<T>");
        }

        [Test]
        public void ShouldHaveSummaryForType()
        {
            var members = new[]
            {
                Type<First>(@"<member name=""T:Example.First""><summary>First summary</summary></member>"),
                Type<Second>(@"<member name=""T:Example.Second""><summary>Second summary</summary></member>"),
            };
            var namespaces = model.Create(members);

            namespaces[0].Types[0].Summary.CountShouldEqual(1);
            ((InlineText)namespaces[0].Types[0].Summary[0]).Text.ShouldEqual("First summary");
            namespaces[0].Types[1].Summary.CountShouldEqual(1);
            ((InlineText)namespaces[0].Types[1].Summary[0]).Text.ShouldEqual("Second summary");
        }

        [Test]
        public void ShouldntHaveAnyUnresolvedReferencesLeftIfAllValid()
        {
            var members = new[]
            {
                Type<First>(@"<member name=""T:Example.First"" />"),  
                Type<Second>(@"<member name=""T:Example.Second""><summary><see cref=""T:Example.First"" /></summary></member>"),  
            };
            var namespaces = model.Create(members);

            ((See)namespaces[0].Types[1].Summary[0]).Reference.ShouldNotBeNull();
            ((See)namespaces[0].Types[1].Summary[0]).Reference.IsResolved.ShouldBeTrue();
        }

        [Test]
        public void UnresolvedReferencesBecomeExternalReferencesIfStillExist()
        {
            var members = new[] { Type<Second>(@"<member name=""T:Example.Second""><summary><see cref=""T:Example.First"" /></summary></member>") };
            var namespaces = model.Create(members);

            ((See)namespaces[0].Types[0].Summary[0]).Reference.IsExternal.ShouldBeTrue();
            ((See)namespaces[0].Types[0].Summary[0]).Reference.Name.ShouldEqual("First");
            ((See)namespaces[0].Types[0].Summary[0]).Reference.FullName.ShouldEqual("Example.First");
        }

        [Test]
        public void ShouldPassSummaryToContentParser()
        {
            var contentParser = MockRepository.GenerateMock<ICommentContentParser>();
            var members = new[] { Type<First>(@"<member name=""T:Example.First""><summary>First summary</summary></member>") };

            contentParser.Stub(x => x.Parse(null))
                .IgnoreArguments()
                .Return(new List<IComment>());
            
            new DocumentModel(contentParser).Create(members);

            contentParser.AssertWasCalled(x => x.Parse(members[0].Xml.ChildNodes[0]));
        }

        [Test]
        public void ShouldForceTypeIfOnlyMethodDefined()
        {
            var members = new[] { Method<Second>(@"<member name=""M:Example.Second.SecondMethod"" />", x => x.SecondMethod()) };
            var namespaces = model.Create(members);

            namespaces[0].Name.ShouldEqual("Example");
            namespaces[0].Types.ShouldContain(x => x.IsIdentifiedBy(Identifier.FromType(typeof(Second))));
        }

        [Test]
        public void ShouldHaveMethodsInTypes()
        {
            var members = new IDocumentationMember[]
            {
                Type<Second>(@"<member name=""T:Example.Second"" />"),  
                Method<Second>(@"<member name=""M:Example.Second.SecondMethod"" />", x => x.SecondMethod()),
                Method<Second>(@"<member name=""M:Example.Second.SecondMethod2(System.String,System.Int32)"" />", x => x.SecondMethod2(null, 0))
            };
            var namespaces = model.Create(members);
            var method = Method<Second>(x => x.SecondMethod());
            var method2 = Method<Second>(x => x.SecondMethod2(null, 0));

            namespaces[0].Types[0].Methods
                .ShouldContain(x => x.IsIdentifiedBy(Identifier.FromMethod(method, typeof(Second))))
                .ShouldContain(x => x.IsIdentifiedBy(Identifier.FromMethod(method2, typeof(Second))));
        }

        [Test]
        public void ShouldHaveSummaryForMethods()
        {
            var members = new[]
            {
                Method<Second>(@"<member name=""M:Example.Second.SecondMethod""><summary>Second method</summary></member>", x => x.SecondMethod()),
                Method<Second>(@"<member name=""M:Example.Second.SecondMethod2(System.String,System.Int32)""><summary>Second method 2</summary></member>", x => x.SecondMethod2(null, 0))
            };
            var namespaces = model.Create(members);

            namespaces[0].Types[0].Methods[0].Summary.CountShouldEqual(1);
            ((InlineText)namespaces[0].Types[0].Methods[0].Summary[0]).Text.ShouldEqual("Second method");
            namespaces[0].Types[0].Methods[1].Summary.CountShouldEqual(1);
            ((InlineText)namespaces[0].Types[0].Methods[1].Summary[0]).Text.ShouldEqual("Second method 2");
        }

        [Test]
        public void ShouldPassMethodSummaryToContentParser()
        {
            var contentParser = MockRepository.GenerateMock<ICommentContentParser>();
            var members = new[] { Method<Second>(@"<member name=""M:Example.Second.SecondMethod""><summary>First summary</summary></member>", x => x.SecondMethod()) };

            contentParser.Stub(x => x.Parse(null))
                .IgnoreArguments()
                .Return(new List<IComment>());

            new DocumentModel(contentParser).Create(members);

            contentParser.AssertWasCalled(x => x.Parse(members[0].Xml.ChildNodes[0]));
        }

        [Test]
        public void ShouldHavePropertiesInTypes()
        {
            var members = new IDocumentationMember[]
            {
                Property<Second>(@"<member name=""P:Example.Second.SecondProperty"" />", x => x.SecondProperty)
            };
            var namespaces = model.Create(members);

            namespaces[0].Types[0].Properties
                .ShouldContain(x => x.Name == "SecondProperty");
        }

        [Test]
        public void ShouldHaveReturnTypeInProperties()
        {
            var members = new IDocumentationMember[]
            {
                Property<Second>(@"<member name=""P:Example.Second.SecondProperty"" />", x => x.SecondProperty)
            };
            var namespaces = model.Create(members);
            var property = namespaces[0].Types[0].Properties[0];

            property.ReturnType.ShouldNotBeNull();
            property.ReturnType.PrettyName.ShouldEqual("string");
        }

        [Test]
        public void ShouldHaveSummaryForProperties()
        {
            var members = new[]
            {
                Property<Second>(@"<member name=""P:Example.Second.SecondProperty""><summary>Second property</summary></member>", x => x.SecondProperty),
            };
            var namespaces = model.Create(members);

            namespaces[0].Types[0].Properties[0].Summary.CountShouldEqual(1);
            ((InlineText)namespaces[0].Types[0].Properties[0].Summary[0]).Text.ShouldEqual("Second property");
        }

        [Test]
        public void ShouldHaveParametersInMethods()
        {
            var members = new IDocumentationMember[]
            {
                Type<First>(@"<member name=""T:Example.First"" />"),
                Method<Second>(@"<member name=""M:Example.Second.SecondMethod2(System.String,Example.First)"" />", x => x.SecondMethod3(null, null))
            };
            var namespaces = model.Create(members);

            var method = namespaces[0].Types[1].Methods[0];

            method.Parameters.CountShouldEqual(2);
            method.Parameters[0].Name.ShouldEqual("one");
            method.Parameters[0].Reference.IsExternal.ShouldBeTrue();
            method.Parameters[1].Name.ShouldEqual("two");
            method.Parameters[1].Reference.ShouldBeOfType<DeclaredType>();
        }

        [Test]
        public void ShouldHaveSummaryForMethodParameter()
        {
            var members = new[]
            {
                Method<Second>(@"
                <member name=""M:Example.Second.SecondMethod2(System.String,System.Int32)"">
                  <param name=""one"">First parameter</param>
                  <param name=""two"">Second parameter</param>
                </member>", x => x.SecondMethod2(null, 0))
            };
            var namespaces = model.Create(members);

            namespaces[0].Types[0].Methods[0].Parameters[0].Summary.CountShouldEqual(1);
            ((InlineText)namespaces[0].Types[0].Methods[0].Parameters[0].Summary[0]).Text.ShouldEqual("First parameter");
            namespaces[0].Types[0].Methods[0].Parameters[1].Summary.CountShouldEqual(1);
            ((InlineText)namespaces[0].Types[0].Methods[0].Parameters[1].Summary[0]).Text.ShouldEqual("Second parameter");
        }

        [Test]
        public void ShouldHaveReturnTypeInMethods()
        {
            var members = new IDocumentationMember[]
            {
                Method<Second>(@"<member name=""M:Example.Second.ReturnType"" />", x => x.ReturnType())
            };
            var namespaces = model.Create(members);
            var method = namespaces[0].Types[0].Methods[0];

            method.ReturnType.ShouldNotBeNull();
            method.ReturnType.PrettyName.ShouldEqual("string");
        }

        [Test]
        public void ShouldPassMethodParameterSummaryToContentParser()
        {
            var contentParser = MockRepository.GenerateMock<ICommentContentParser>();
            var members = new[]
            {
                Method<Second>(@"
                <member name=""M:Example.Second.SecondMethod2(System.String,System.Int32)"">
                  <param name=""one"">First parameter</param>
                  <param name=""two"">Second parameter</param>
                </member>", x => x.SecondMethod2(null, 0))
            };

            contentParser.Stub(x => x.Parse(null))
                .IgnoreArguments()
                .Return(new List<IComment>());

            new DocumentModel(contentParser).Create(members);

            contentParser.AssertWasCalled(x => x.Parse(members[0].Xml.ChildNodes[0]));
            contentParser.AssertWasCalled(x => x.Parse(members[0].Xml.ChildNodes[1]));
        }
    }
}