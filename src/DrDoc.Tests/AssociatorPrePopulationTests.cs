using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using DrDoc.Model;
using DrDoc.Parsing;
using Example;
using NUnit.Framework;

namespace DrDoc.Tests
{
    [TestFixture]
    public class AssociatorPrePopulationTests : BaseFixture
    {
        private Associator associator;

        [SetUp]
        public void CreateAssociator()
        {
            associator = new Associator();
        }

        [Test]
        public void ShouldAddClass()
        {
            var associations = associator.AssociateMembersWithXml(DocMembers(typeof(EmptyType)), new XmlNode[0]);

            var member = associations.FirstOrDefault(x => x.Name == Identifier.FromType(typeof(EmptyType)));
            member.ShouldBeOfType<UndocumentedType>();
            member.Name.ToString().ShouldEqual("EmptyType");
        }

        [Test]
        public void ShouldAddClassMethods()
        {
            var associations = associator.AssociateMembersWithXml(DocMembers(typeof(SingleMethodType)), new XmlNode[0]);

            var member = associations.FirstOrDefault(x => x.Name == Identifier.FromMethod(typeof(SingleMethodType).GetMethod("Method"), typeof(SingleMethodType)));
            member.ShouldBeOfType<UndocumentedMethod>();
            member.Name.ToString().ShouldEqual("Method");
        }

        [Test]
        public void ShouldAddOverloadedClassMethods()
        {
            var associations = associator.AssociateMembersWithXml(DocMembers(typeof(ClassWithOverload)), new XmlNode[0]);
            var method1 = Method<ClassWithOverload>(x => x.Method());
            var method2 = Method<ClassWithOverload>(x => x.Method(null));

            var member = associations.FirstOrDefault(x => x.Name == Identifier.FromMethod(method1, typeof(ClassWithOverload)));
            member.ShouldBeOfType<UndocumentedMethod>();
            member.Name.ToString().ShouldEqual("Method");

            var member2 = associations.FirstOrDefault(x => x.Name == Identifier.FromMethod(method2, typeof(ClassWithOverload)));
            member2.ShouldBeOfType<UndocumentedMethod>();
            member2.Name.ToString().ShouldEqual("Method");

            member2.ShouldNotEqual(member);
        }

        [Test]
        public void ShouldntAddPropertyMethods()
        {
            var associations = associator.AssociateMembersWithXml(DocMembers(typeof(PropertyType)), new XmlNode[0]);

            var member = associations.FirstOrDefault(x => x.Name == Identifier.FromMethod(typeof(PropertyType).GetMethod("get_Property"), typeof(PropertyType)));
            member.ShouldBeNull();
        }

        [Test, Ignore]
        public void ShouldAddExplicitlyImplementedClassMethods()
        {
            var associations = associator.AssociateMembersWithXml(DocMembers(typeof(SingleMethodType)), new XmlNode[0]);

            var member = associations.FirstOrDefault(x => x.Name == Identifier.FromMethod(typeof(ClassWithExplicitMethodImplementation).GetMethod("Method"), typeof(ClassWithExplicitMethodImplementation)));
            member.ShouldBeOfType<UndocumentedMethod>();
            member.Name.ToString().ShouldEqual("Method");
        }

        [Test]
        public void ShouldAddInterface()
        {
            var associations = associator.AssociateMembersWithXml(DocMembers(typeof(EmptyInterface)), new XmlNode[0]);

            var member = associations.FirstOrDefault(x => x.Name == Identifier.FromType(typeof(EmptyInterface)));
            member.ShouldBeOfType<UndocumentedType>();
            member.Name.ToString().ShouldEqual("EmptyInterface");
        }

        [Test]
        public void ShouldAddInterfaceMethods()
        {
            var associations = associator.AssociateMembersWithXml(DocMembers(typeof(SingleMethodInterface)), new XmlNode[0]);

            var member = associations.FirstOrDefault(x => x.Name == Identifier.FromMethod(typeof(SingleMethodInterface).GetMethod("Method"), typeof(SingleMethodInterface)));
            member.ShouldBeOfType<UndocumentedMethod>();
            member.Name.ToString().ShouldEqual("Method");
        }
    }
}