using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml;
using DrDoc.Parsing.Model;
using DrDoc.Parsing;
using Example;
using NUnit.Framework;

namespace DrDoc.Tests.Parsing
{
    [TestFixture]
    public class DocumentationXmlMatcherPrePopulationTests : BaseFixture
    {
        private DocumentationXmlMatcher matcher;
        private IList<IDocumentationMember> members;

        [SetUp]
        public void CreateAssociator()
        {
            matcher = new DocumentationXmlMatcher();
        }

        [Test]
        public void ShouldAddClass()
        {
            document_member<EmptyType>();

            var member = find_member<EmptyType>();
            member.ShouldBeOfType<UndocumentedType>();
            member.Name.ToString().ShouldEqual("EmptyType");
        }

        [Test]
        public void ShouldAddClassMethods()
        {
            document_member<SingleMethodType>();

            var member = find_member<SingleMethodType>(x => x.Method());
            member.ShouldBeOfType<UndocumentedMethod>();
            member.Name.ToString().ShouldEqual("Method");
        }

        [Test]
        public void ShouldAddOverloadedClassMethods()
        {
            document_member<ClassWithOverload>();

            var member = find_member<ClassWithOverload>(x => x.Method());
            member.ShouldBeOfType<UndocumentedMethod>();
            member.Name.ToString().ShouldEqual("Method");

            var member2 = find_member<ClassWithOverload>(x => x.Method(null));
            member2.ShouldBeOfType<UndocumentedMethod>();
            member2.Name.ToString().ShouldEqual("Method");
            member2.ShouldNotEqual(member);
        }

        [Test]
        public void ShouldntAddPropertyMethods()
        {
            document_member<PropertyType>();

            find_member<PropertyType>("get_Property")
                .ShouldBeNull();
        }

        [Test, Ignore]
        public void ShouldAddExplicitlyImplementedClassMethods()
        {
            document_member<ClassWithExplicitMethodImplementation>();
            var member = find_member<ClassWithExplicitMethodImplementation>("Method");

            member.ShouldBeOfType<UndocumentedMethod>();
            member.Name.ToString().ShouldEqual("Method");
        }

        [Test]
        public void ShouldAddInterface()
        {
            document_member<EmptyInterface>();
            var member = find_member<EmptyInterface>();

            member.ShouldBeOfType<UndocumentedType>();
            member.Name.ToString().ShouldEqual("EmptyInterface");
        }

        [Test]
        public void ShouldAddInterfaceMethods()
        {
            document_member<SingleMethodInterface>();
            var member = find_member<SingleMethodInterface>(x => x.Method());

            member.ShouldBeOfType<UndocumentedMethod>();
            member.Name.ToString().ShouldEqual("Method");
        }

        [Test]
        public void should_add_static_methods()
        {
            document_member<StaticMethodClass>();
            var member = find_member<StaticMethodClass>(() => StaticMethodClass.Method());

            member.ShouldBeOfType<UndocumentedMethod>();
            member.Name.ToString().ShouldEqual("Method");
        }

        private void document_member<T>()
        {
            members = matcher.DocumentMembers(DocMembers(typeof(T)), new XmlNode[0]);
        }

        private IDocumentationMember find_member<T>()
        {
            return members.FirstOrDefault(x => x.Name == Identifier.FromType(typeof(T)));
        }

        private IDocumentationMember find_member<T>(Expression<Action<T>> methodAction)
        {
            var method = ((MethodCallExpression)methodAction.Body).Method;

            return members.FirstOrDefault(x => x.Name == Identifier.FromMethod(method, typeof(T)));
        }

        private IDocumentationMember find_member<T>(Expression<Action> methodAction)
        {
            var method = ((MethodCallExpression)methodAction.Body).Method;

            return members.FirstOrDefault(x => x.Name == Identifier.FromMethod(method, typeof(T)));
        }

        private IDocumentationMember find_member<T>(string methodName)
        {
            var method = typeof(T).GetMethod(methodName);

            return members.FirstOrDefault(x => x.Name == Identifier.FromMethod(method, typeof(T)));
        }
    }
}