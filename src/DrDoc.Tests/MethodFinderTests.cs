using System;
using DrDoc.Utils;
using NUnit.Framework;

namespace DrDoc.Tests
{
    [TestFixture]
    public class MethodFinderTests
    {
        [Test]
        public void Find()
        {
            var method = Method.Find(typeof(Target), "VoidNoArgs", new Type[0]);

            method.ShouldNotBeNull();
            method.Name.ShouldEqual("VoidNoArgs");
            method.IsGenericMethod.ShouldBeFalse();
            method.IsGenericMethodDefinition.ShouldBeFalse();
            method.GetParameters().CountShouldEqual(0);
            method.GetGenericArguments().CountShouldEqual(0);
        }

        [Test]
        public void FindGeneric()
        {
            var method = Method.Find(typeof(Target), "VoidNoArgs``1", new Type[0]);

            method.ShouldNotBeNull();
            method.Name.ShouldEqual("VoidNoArgs");
            method.IsGenericMethod.ShouldBeTrue();
            method.IsGenericMethodDefinition.ShouldBeTrue();
            method.GetParameters().CountShouldEqual(0);
            method.GetGenericArguments().CountShouldEqual(1);
        }

        private class Target
        {
            public void VoidNoArgs()
            {
                
            }

            public void VoidNoArgs<T>()
            {

            }
        }
    }
}