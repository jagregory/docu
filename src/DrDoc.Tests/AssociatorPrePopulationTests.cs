using System;
using System.Linq;
using System.Xml;
using DrDoc.Associations;
using NUnit.Framework;

namespace DrDoc.Tests
{
    [TestFixture]
    public class AssociatorPrePopulationTests
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

    public class EmptyType
    {}

    public class SingleMethodType
    {
        public void Method()
        {}
    }

    public class PropertyType
    {
        public string Property { get; set; }
    }

    public class ClassWithExplicitMethodImplementation : InterfaceForExplicitImplementation
    {
        void InterfaceForExplicitImplementation.Method()
        {
            
        }
    }

    public interface EmptyInterface
    {}

    public interface SingleMethodInterface
    {
        void Method();
    }

    public interface InterfaceForExplicitImplementation
    {
        void Method();
    }
}