using System;
using System.Linq;
using System.Xml;
using DrDoc.Associations;
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
            var associations = associator.Examine(new[] {typeof(EmptyType)}, new XmlNode[0]);

            var member = associations.FirstOrDefault(x => x.Name == MemberName.FromType(typeof(EmptyType)));
            member.ShouldBeOfType<UndocumentedTypeAssociation>();
            member.Name.ToString().ShouldEqual("EmptyType");
        }

        [Test]
        public void ShouldAddClassMethods()
        {
            var associations = associator.Examine(new[] { typeof(SingleMethodType) }, new XmlNode[0]);

            var member = associations.FirstOrDefault(x => x.Name == MemberName.FromMethod(typeof(SingleMethodType).GetMethod("Method"), typeof(SingleMethodType)));
            member.ShouldBeOfType<UndocumentedMethodAssociation>();
            member.Name.ToString().ShouldEqual("Method");
        }

        [Test]
        public void ShouldAddOverloadedClassMethods()
        {
            var associations = associator.Examine(new[] { typeof(ClassWithOverload) }, new XmlNode[0]);
            var method1 = Method<ClassWithOverload>(x => x.Method());
            var method2 = Method<ClassWithOverload>(x => x.Method(null));

            var member = associations.FirstOrDefault(x => x.Name == MemberName.FromMethod(method1, typeof(ClassWithOverload)));
            member.ShouldBeOfType<UndocumentedMethodAssociation>();
            member.Name.ToString().ShouldEqual("Method");

            var member2 = associations.FirstOrDefault(x => x.Name == MemberName.FromMethod(method2, typeof(ClassWithOverload)));
            member2.ShouldBeOfType<UndocumentedMethodAssociation>();
            member2.Name.ToString().ShouldEqual("Method");

            member2.ShouldNotEqual(member);
        }

        [Test]
        public void ShouldntAddPropertyMethods()
        {
            var associations = associator.Examine(new[] { typeof(PropertyType) }, new XmlNode[0]);

            var member = associations.FirstOrDefault(x => x.Name == MemberName.FromMethod(typeof(PropertyType).GetMethod("get_Property"), typeof(PropertyType)));
            member.ShouldBeNull();
        }

        [Test, Ignore]
        public void ShouldAddExplicitlyImplementedClassMethods()
        {
            var associations = associator.Examine(new[] { typeof(SingleMethodType) }, new XmlNode[0]);

            var member = associations.FirstOrDefault(x => x.Name == MemberName.FromMethod(typeof(ClassWithExplicitMethodImplementation).GetMethod("Method"), typeof(ClassWithExplicitMethodImplementation)));
            member.ShouldBeOfType<UndocumentedMethodAssociation>();
            member.Name.ToString().ShouldEqual("Method");
        }

        [Test]
        public void ShouldAddInterface()
        {
            var associations = associator.Examine(new[] { typeof(EmptyInterface) }, new XmlNode[0]);

            var member = associations.FirstOrDefault(x => x.Name == MemberName.FromType(typeof(EmptyInterface)));
            member.ShouldBeOfType<UndocumentedTypeAssociation>();
            member.Name.ToString().ShouldEqual("EmptyInterface");
        }

        [Test]
        public void ShouldAddInterfaceMethods()
        {
            var associations = associator.Examine(new[] { typeof(SingleMethodInterface) }, new XmlNode[0]);

            var member = associations.FirstOrDefault(x => x.Name == MemberName.FromMethod(typeof(SingleMethodInterface).GetMethod("Method"), typeof(SingleMethodInterface)));
            member.ShouldBeOfType<UndocumentedMethodAssociation>();
            member.Name.ToString().ShouldEqual("Method");
        }
    }
}