using System.Linq;
using DrDoc.Parsing.Model;
using DrDoc.Parsing;
using Example;
using NUnit.Framework;

namespace DrDoc.Tests.Parsing
{
    [TestFixture]
    public class DocumentationXmlMatcherTests : BaseFixture
    {
        private DocumentationXmlMatcher matcher;

        [SetUp]
        public void CreateAssociator()
        {
            matcher = new DocumentationXmlMatcher();
        }

        [Test]
        public void ShouldAssociateTypeSnippetWithCorrectReflectedType()
        {
            var undocumentedMembers = DocMembers(typeof(First), typeof(Second), typeof(Third));
            var snippets = new[] { @"<member name=""T:Example.Second"" />".ToNode()};
            var members = matcher.DocumentMembers(undocumentedMembers, snippets);

            var member = members.FirstOrDefault(x => x.Name == Identifier.FromType(typeof(Second))) as DocumentedType;

            member.ShouldNotBeNull();
            member.Xml.ShouldEqual(snippets[0]);
            member.Type.ShouldEqual(typeof(Second));
        }

        [Test]
        public void ShouldAssociateGenericTypeSnippetWithCorrectReflectedType()
        {
            var undocumentedMembers = DocMembers(typeof(First), typeof(GenericDefinition<>));
            var snippets = new[] { @"<member name=""T:Example.GenericDefinition`1"" />".ToNode() };
            var members = matcher.DocumentMembers(undocumentedMembers, snippets);

            var member = members.FirstOrDefault(x => x.Name == Identifier.FromType(typeof(GenericDefinition<>))) as DocumentedType;

            member.ShouldNotBeNull();
            member.Xml.ShouldEqual(snippets[0]);
            member.Type.ShouldEqual(typeof(GenericDefinition<>));
        }

        [Test]
        public void ShouldAssociateGenericTypeWithMultipleParamsSnippetWithCorrectReflectedType()
        {
            var undocumentedMembers = DocMembers(typeof(First), typeof(GenericDefinition<>), typeof(GenericDefinition<,>));
            var snippets = new[] { @"<member name=""T:Example.GenericDefinition`2"" />".ToNode() };
            var members = matcher.DocumentMembers(undocumentedMembers, snippets);

            var member = members.FirstOrDefault(x => x.Name == Identifier.FromType(typeof(GenericDefinition<,>))) as DocumentedType;

            member.ShouldNotBeNull();
            member.Xml.ShouldEqual(snippets[0]);
            member.Type.ShouldEqual(typeof(GenericDefinition<,>));
        }

        [Test]
        public void ShouldAssociateMethodSnippetWithCorrectReflectedMethod()
        {
            var undocumentedMembers = DocMembers(typeof(First), typeof(Second), typeof(Third));
            var snippets = new[] { @"<member name=""M:Example.Second.SecondMethod"" />".ToNode() };
            var members = matcher.DocumentMembers(undocumentedMembers, snippets);
            var method = Method<Second>(x => x.SecondMethod());

            var member = members.FirstOrDefault(x => x.Name == Identifier.FromMethod(method, typeof(Second))) as DocumentedMethod;

            member.ShouldNotBeNull();
            member.Xml.ShouldEqual(snippets[0]);
            member.Method.ShouldEqual<Second>(x => x.SecondMethod());
        }

        [Test]
        public void ShouldAssociateMethodOverloadSnippetWithCorrectReflectedMethod()
        {
            var undocumentedMembers = DocMembers(typeof(ClassWithOverload));
            var snippets = new[] { @"<member name=""M:Example.ClassWithOverload.Method"" />".ToNode(), @"<member name=""M:Example.ClassWithOverload.Method(System.String)"" />".ToNode() };
            var members = matcher.DocumentMembers(undocumentedMembers, snippets);
            var method = Method<ClassWithOverload>(x => x.Method());
            var method2 = Method<ClassWithOverload>(x => x.Method(null));

            var member = members.FirstOrDefault(x => x.Name == Identifier.FromMethod(method, typeof(ClassWithOverload))) as DocumentedMethod;
            member.ShouldNotBeNull();
            member.Xml.ShouldEqual(snippets[0]);
            member.Method.ShouldEqual(method);

            var member2 = members.FirstOrDefault(x => x.Name == Identifier.FromMethod(method2, typeof(ClassWithOverload))) as DocumentedMethod;
            member2.ShouldNotBeNull();
            member2.Xml.ShouldEqual(snippets[0]);
            member2.Method.ShouldEqual(method2);
        }

        [Test]
        public void ShouldAssociateMethodOnGenericTypeSnippetWithCorrectReflectedMethod()
        {
            var undocumentedMembers = DocMembers(typeof(First), typeof(GenericDefinition<>), typeof(GenericDefinition<,>));
            var snippets = new[] { @"<member name=""M:Example.GenericDefinition`1.AMethod"" />".ToNode() };
            var members = matcher.DocumentMembers(undocumentedMembers, snippets);
            var method = Method<GenericDefinition<object>>(x => x.AMethod());

            var member = members.FirstOrDefault(x => x.Name == Identifier.FromMethod(method, typeof(GenericDefinition<>))) as DocumentedMethod;

            member.ShouldNotBeNull();
            member.Xml.ShouldEqual(snippets[0]);
            member.Method.Name.ShouldEqual("AMethod");
            member.Method.IsGenericMethod.ShouldBeFalse();
        }

        [Test]
        public void ShouldAssociateGenericMethodOnGenericTypeSnippetWithCorrectReflectedMethod()
        {
            var undocumentedMembers = DocMembers(typeof(First), typeof(GenericDefinition<>), typeof(GenericDefinition<,>));
            var snippets = new[] { @"<member name=""M:Example.GenericDefinition`1.BMethod``1"" />".ToNode() };
            var members = matcher.DocumentMembers(undocumentedMembers, snippets);
            var method = typeof(GenericDefinition<>).GetMethod("BMethod");

            var member = members.FirstOrDefault(x => x.Name == Identifier.FromMethod(method, typeof(GenericDefinition<>))) as DocumentedMethod;

            member.ShouldNotBeNull();
            member.Xml.ShouldEqual(snippets[0]);
            member.Method.ShouldBeSameAs(method);
        }

        [Test]
        public void ShouldAssociateMethodWithParametersSnippetWithCorrectReflectedMethod()
        {
            var undocumentedMembers = DocMembers(typeof(First), typeof(Second), typeof(Third));
            var snippets = new[] { @"<member name=""M:Example.Second.SecondMethod2(System.String,System.Int32)"" />".ToNode() };
            var members = matcher.DocumentMembers(undocumentedMembers, snippets);
            var method = Method<Second>(x => x.SecondMethod2(null, 0));

            var member = members.FirstOrDefault(x => x.Name == Identifier.FromMethod(method, typeof(Second))) as DocumentedMethod;

            member.ShouldNotBeNull();
            member.Xml.ShouldEqual(snippets[0]);
            member.Method.ShouldEqual<Second>(x => x.SecondMethod2("", 0));
        }

        [Test]
        public void ShouldAssociatePropertySnippetWithCorrectReflectedProperty()
        {
            var undocumentedMembers = DocMembers(typeof(First), typeof(Second), typeof(Third));
            var snippets = new[] { @"<member name=""P:Example.Second.SecondProperty"" />".ToNode() };
            var members = matcher.DocumentMembers(undocumentedMembers, snippets);
            var property = Property<Second>(x => x.SecondProperty);

            var member = members.FirstOrDefault(x => x.Name == Identifier.FromProperty(property, typeof(Second))) as DocumentedProperty;

            member.ShouldNotBeNull();
            member.Xml.ShouldEqual(snippets[0]);
            member.Property.ShouldEqual<Second>(x => x.SecondProperty);
        }
    }
}