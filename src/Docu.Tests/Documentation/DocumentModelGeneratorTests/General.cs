using System.Collections.Generic;
using Docu.Documentation;
using Docu.Documentation.Comments;
using Docu.Parsing.Comments;
using Docu.Parsing.Model;
using Example;
using Example.Deep;
using NUnit.Framework;

namespace Docu.Tests.Documentation.DocumentModelGeneratorTests
{
    [TestFixture]
    public class General : BaseDocumentModelGeneratorFixture
    {
        [Test]
        public void ShouldHaveTypesInNamespaces()
        {
            var model = new DocumentModel(StubParser, StubEventAggregator);
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
            var model = new DocumentModel(StubParser, StubEventAggregator);
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
            var model = new DocumentModel(StubParser, StubEventAggregator);
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
            var model = new DocumentModel(StubParser, StubEventAggregator);
            var members = new[]
            {
                Type<ClassWithInterfaces>(@"<member name=""T:Example.ClassWithInterfaces"" />"),  
            };
            var namespaces = model.Create(members);

            namespaces[0].Types[0].Interfaces.Count.ShouldEqual(2);
            namespaces[0].Types[0].Interfaces[0].PrettyName.ShouldEqual("EmptyInterface");
            namespaces[0].Types[0].Interfaces[1].PrettyName.ShouldEqual("IDisposable");
        }

        [Test]
        public void ShouldntInheritInterfacesForTypes()
        {
            var model = new DocumentModel(StubParser, StubEventAggregator);
            var members = new[]
            {
                Type<ClassWithBaseWithInterfaces>(@"<member name=""T:Example.ClassWithBaseWithInterfaces"" />"),  
            };
            var namespaces = model.Create(members);

            namespaces[0].Types[0].Interfaces.Count.ShouldEqual(0);
        }

        [Test]
        public void ShouldntShowOnlyDirectInterfacesForTypes()
        {
            var model = new DocumentModel(StubParser, StubEventAggregator);
            var members = new[]
            {
                Type<ClassWithBaseWithInterfacesAndDirect>(@"<member name=""T:Example.ClassWithBaseWithInterfacesAndDirect"" />"),  
            };
            var namespaces = model.Create(members);

            namespaces[0].Types[0].Interfaces.Count.ShouldEqual(1);
            namespaces[0].Types[0].Interfaces[0].PrettyName.ShouldEqual("IExample");
        }

        [Test]
        public void ShouldHavePrettyNamesForGenericTypes()
        {
            var model = new DocumentModel(StubParser, StubEventAggregator);
            var members = new[] { Type(typeof(GenericDefinition<>), @"<member name=""T:Example.GenericDefinition`1"" />") };
            var namespaces = model.Create(members);

            namespaces[0].Types
                .ShouldContain(x => x.PrettyName == "GenericDefinition<T>");
        }

        [Test]
        public void ShouldntHaveAnyUnresolvedReferencesLeftIfAllValid()
        {
            var model = new DocumentModel(new CommentParser(), StubEventAggregator);
            var members = new[]
            {
                Type<First>(@"<member name=""T:Example.First"" />"),  
                Type<Second>(@"<member name=""T:Example.Second""><summary><see cref=""T:Example.First"" /></summary></member>"),  
            };
            var namespaces = model.Create(members);
            var comment = new List<IComment>(namespaces[0].Types[1].Summary.Children);

            ((See)comment[0]).Reference.ShouldNotBeNull();
            ((See)comment[0]).Reference.IsResolved.ShouldBeTrue();
        }

        [Test]
        public void UnresolvedReferencesBecomeExternalReferencesIfStillExist()
        {
            var model = new DocumentModel(new CommentParser(), StubEventAggregator);
            var members = new[] { Type<Second>(@"<member name=""T:Example.Second""><summary><see cref=""T:Example.First"" /></summary></member>") };
            var namespaces = model.Create(members);
            var comment = new List<IComment>(namespaces[0].Types[0].Summary.Children);

            ((See)comment[0]).Reference.IsExternal.ShouldBeTrue();
            ((See)comment[0]).Reference.Name.ShouldEqual("First");
            ((See)comment[0]).Reference.FullName.ShouldEqual("Example.First");
        }

        [Test]
        public void ShouldForceTypeIfOnlyMethodDefined()
        {
            var model = new DocumentModel(StubParser, StubEventAggregator);
            var members = new[] { Method<Second>(@"<member name=""M:Example.Second.SecondMethod"" />", x => x.SecondMethod()) };
            var namespaces = model.Create(members);

            namespaces[0].Name.ShouldEqual("Example");
            namespaces[0].Types.ShouldContain(x => x.IsIdentifiedBy(Identifier.FromType(typeof(Second))));
        }

        [Test]
        public void ShouldHaveMethodsInTypes()
        {
            var model = new DocumentModel(StubParser, StubEventAggregator);
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
        public void ShouldHavePropertiesInTypes()
        {
            var model = new DocumentModel(StubParser, StubEventAggregator);
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
            var model = new DocumentModel(StubParser, StubEventAggregator);
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
        public void ShouldHaveReturnTypeInFields()
        {
            var model = new DocumentModel(StubParser, StubEventAggregator);
            var members = new IDocumentationMember[]
            {
                Field<Second>(@"<member name=""F:Example.Second.aField"" />", x => x.aField)
            };
            var namespaces = model.Create(members);
            var field = namespaces[0].Types[0].Fields[0];

            field.ReturnType.ShouldNotBeNull();
            field.ReturnType.PrettyName.ShouldEqual("string");
        }

        [Test]
        public void ShouldHaveParametersInMethods()
        {
            var model = new DocumentModel(StubParser, StubEventAggregator);
            var members = new IDocumentationMember[]
            {
                Type<First>(@"<member name=""T:Example.First"" />"),
                Method<Second>(@"<member name=""M:Example.Second.SecondMethod2(System.String,Example.First)"" />", x => x.SecondMethod3(null, null))
            };
            var namespaces = model.Create(members);

            var method = namespaces[0].Types[1].Methods[0];

            method.Parameters.Count.ShouldEqual(2);
            method.Parameters[0].Name.ShouldEqual("one");
            method.Parameters[0].Reference.IsExternal.ShouldBeTrue();
            method.Parameters[1].Name.ShouldEqual("two");
            method.Parameters[1].Reference.ShouldBeOfType<DeclaredType>();
        }

        [Test]
        public void ShouldHaveReturnTypeInMethods()
        {
            var model = new DocumentModel(StubParser, StubEventAggregator);
            var members = new IDocumentationMember[]
            {
                Method<Second>(@"<member name=""M:Example.Second.ReturnType"" />", x => x.ReturnType())
            };
            var namespaces = model.Create(members);
            var method = namespaces[0].Types[0].Methods[0];

            method.ReturnType.ShouldNotBeNull();
            method.ReturnType.PrettyName.ShouldEqual("string");
        }
    }
}