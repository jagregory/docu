using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml;
using Docu.Parsing;
using Docu.Parsing.Model;
using Example;
using NUnit.Framework;

namespace Docu.Tests.Parsing
{
    [TestFixture]
    public class DocumentationXmlMatcherPrePopulationTests : BaseFixture
    {
        private IList<IDocumentationMember> members;

        [SetUp]
        public void CreateAssociator()
        {
        }

        [Test]
        public void ShouldAddClass()
        {
            document_member<EmptyType>();

            var member = find_member<EmptyType>();
            member.ShouldBeOfType<ReflectedType>();
            member.Name.ToString().ShouldEqual("EmptyType");
        }

        [Test]
        public void ShouldAddClassMethods()
        {
            document_member<SingleMethodType>();

            var member = find_member<SingleMethodType>(x => x.Method());
            member.ShouldBeOfType<ReflectedMethod>();
            member.Name.ToString().ShouldEqual("Method");
        }

        [Test]
        public void ShouldAddOverloadedClassMethods()
        {
            document_member<ClassWithOverload>();

            var member = find_member<ClassWithOverload>(x => x.Method());
            member.ShouldBeOfType<ReflectedMethod>();
            member.Name.ToString().ShouldEqual("Method");

            var member2 = find_member<ClassWithOverload>(x => x.Method(null));
            member2.ShouldBeOfType<ReflectedMethod>();
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

            member.ShouldBeOfType<ReflectedMethod>();
            member.Name.ToString().ShouldEqual("Method");
        }

        [Test]
        public void ShouldAddInterface()
        {
            document_member<EmptyInterface>();
            var member = find_member<EmptyInterface>();

            member.ShouldBeOfType<ReflectedType>();
            member.Name.ToString().ShouldEqual("EmptyInterface");
        }

        [Test]
        public void ShouldAddInterfaceMethods()
        {
            document_member<SingleMethodInterface>();
            var member = find_member<SingleMethodInterface>(x => x.Method());

            member.ShouldBeOfType<ReflectedMethod>();
            member.Name.ToString().ShouldEqual("Method");
        }

        [Test]
        public void should_add_static_methods()
        {
            document_member<StaticMethodClass>();
            var member = find_member<StaticMethodClass>(() => StaticMethodClass.Method());

            member.ShouldBeOfType<ReflectedMethod>();
            member.Name.ToString().ShouldEqual("Method");
        }

        [Test]
        public void should_add_events()
        {
            document_member<EventTypeEx>();

            var member = find_event<EventTypeEx>("AnEvent");
            member.ShouldBeOfType<ReflectedEvent>();
            member.Name.ToString().ShouldEqual("AnEvent");
        }

        [Test]
        public void should_add_fields()
        {
            document_member<FieldType>();

            var member = find_member<FieldType>(x => x.aField);
            member.ShouldBeOfType<ReflectedField>();
            member.Name.ToString().ShouldEqual("aField");
        }

        private void document_member<T>()
        {
            members = DocumentationXmlMatcher.MatchDocumentationToMembers(DocumentableMemberFinder.ReflectMembersForDocumenting(new[] {typeof(T)}), new XmlNode[0]);
        }

        private IDocumentationMember find_member<T>()
        {
            return members.FirstOrDefault(x => x.Name == IdentifierFor.Type(typeof(T)));
        }

        private IDocumentationMember find_event<T>(string name)
        {
            return members.FirstOrDefault(x => x.Name == IdentifierFor.Event(typeof(T).GetEvent(name), typeof(T)));
        }

        private IDocumentationMember find_member<T>(Expression<Action<T>> methodAction)
        {
            var method = ((MethodCallExpression)methodAction.Body).Method;

            return members.FirstOrDefault(x => x.Name == IdentifierFor.Method(method, typeof(T)));
        }

        private IDocumentationMember find_member<T>(Expression<Func<T, object>> propertyOrField)
        {
            var member = ((MemberExpression)propertyOrField.Body).Member;

            if (member is PropertyInfo)
                return members.FirstOrDefault(x => x.Name == IdentifierFor.Property((PropertyInfo)member, typeof(T)));

            return members.FirstOrDefault(x => x.Name == IdentifierFor.Field((FieldInfo)member, typeof(T)));
        }

        private IDocumentationMember find_member<T>(Expression<Action> methodAction)
        {
            var method = ((MethodCallExpression)methodAction.Body).Method;

            return members.FirstOrDefault(x => x.Name == IdentifierFor.Method(method, typeof(T)));
        }

        private IDocumentationMember find_member<T>(string methodName)
        {
            var method = typeof(T).GetMethod(methodName);

            return members.FirstOrDefault(x => x.Name == IdentifierFor.Method(method, typeof(T)));
        }
    }
}