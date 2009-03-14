using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DrDoc.Associations;
using DrDoc.Utils;
using Example;
using NUnit.Framework;

namespace DrDoc.Tests
{
    [TestFixture]
    public class AssociationTests
    {
        private Associator associator;

        [SetUp]
        public void CreateAssociator()
        {
            associator = new Associator();
        }

        private MethodInfo Method<T>(Expression<Action<T>> methodAction)
        {
            var method = ((MethodCallExpression)methodAction.Body).Method;

            if (method.IsGenericMethod)
                return method.GetGenericMethodDefinition();

            return method;
        }

        private PropertyInfo Property<T>(Expression<Func<T, object>> propertyAction)
        {
            return ((MemberExpression)propertyAction.Body).Member as PropertyInfo;
        }

        [Test]
        public void ShouldAssociateTypeSnippetWithCorrectReflectedType()
        {
            var types = new[] {typeof(First), typeof(Second), typeof(Third)};
            var snippets = new[] { @"<member name=""T:Example.Second"" />".ToNode()};
            var associations = associator.Examine(types, snippets);

            var ass = associations.FirstOrDefault(x => x.Name == MemberName.FromType(typeof(Second))) as TypeAssociation;

            ass.ShouldNotBeNull();
            ass.Xml.ShouldEqual(snippets[0]);
            ass.Type.ShouldEqual(typeof(Second));
        }

        [Test]
        public void ShouldAssociateGenericTypeSnippetWithCorrectReflectedType()
        {
            var types = new[] { typeof(First), typeof(GenericDefinition<>) };
            var snippets = new[] { @"<member name=""T:Example.GenericDefinition`1"" />".ToNode() };
            var associations = associator.Examine(types, snippets);

            var ass = associations.FirstOrDefault(x => x.Name == MemberName.FromType(typeof(GenericDefinition<>))) as TypeAssociation;

            ass.ShouldNotBeNull();
            ass.Xml.ShouldEqual(snippets[0]);
            ass.Type.ShouldEqual(typeof(GenericDefinition<>));
        }

        [Test]
        public void ShouldAssociateGenericTypeWithMultipleParamsSnippetWithCorrectReflectedType()
        {
            var types = new[] { typeof(First), typeof(GenericDefinition<>), typeof(GenericDefinition<,>) };
            var snippets = new[] { @"<member name=""T:Example.GenericDefinition`2"" />".ToNode() };
            var associations = associator.Examine(types, snippets);

            var ass = associations.FirstOrDefault(x => x.Name == MemberName.FromType(typeof(GenericDefinition<,>))) as TypeAssociation;

            ass.ShouldNotBeNull();
            ass.Xml.ShouldEqual(snippets[0]);
            ass.Type.ShouldEqual(typeof(GenericDefinition<,>));
        }

        [Test]
        public void ShouldAssociateMethodSnippetWithCorrectReflectedMethod()
        {
            var types = new[] { typeof(First), typeof(Second), typeof(Third) };
            var snippets = new[] { @"<member name=""M:Example.Second.SecondMethod"" />".ToNode() };
            var associations = associator.Examine(types, snippets);
            var method = Method<Second>(x => x.SecondMethod());

            var ass = associations.FirstOrDefault(x => x.Name == MemberName.FromMethod(method, typeof(Second))) as MethodAssociation;

            ass.ShouldNotBeNull();
            ass.Xml.ShouldEqual(snippets[0]);
            ass.Method.ShouldEqual<Second>(x => x.SecondMethod());
        }

        [Test]
        public void ShouldAssociateMethodOnGenericTypeSnippetWithCorrectReflectedMethod()
        {
            var types = new[] { typeof(First), typeof(GenericDefinition<>), typeof(GenericDefinition<,>) };
            var snippets = new[] { @"<member name=""M:Example.GenericDefinition`1.AMethod"" />".ToNode() };
            var associations = associator.Examine(types, snippets);
            var method = Method<GenericDefinition<object>>(x => x.AMethod());

            var ass = associations.FirstOrDefault(x => x.Name == MemberName.FromMethod(method, typeof(GenericDefinition<>))) as MethodAssociation;

            ass.ShouldNotBeNull();
            ass.Xml.ShouldEqual(snippets[0]);
            ass.Method.Name.ShouldEqual("AMethod");
            ass.Method.IsGenericMethod.ShouldBeFalse();
        }

        [Test]
        public void ShouldAssociateGenericMethodOnGenericTypeSnippetWithCorrectReflectedMethod()
        {
            var types = new[] { typeof(First), typeof(GenericDefinition<>), typeof(GenericDefinition<,>) };
            var snippets = new[] { @"<member name=""M:Example.GenericDefinition`1.BMethod``1"" />".ToNode() };
            var associations = associator.Examine(types, snippets);
            var method = typeof(GenericDefinition<>).GetMethod("BMethod");

            // may be broke!
            var ass = associations.FirstOrDefault(x => x.Name == MemberName.FromMethod(method, typeof(GenericDefinition<>))) as MethodAssociation;

            ass.ShouldNotBeNull();
            ass.Xml.ShouldEqual(snippets[0]);
            ass.Method.ShouldBeSameAs(method);
        }

        [Test]
        public void ShouldAssociateMethodWithParametersSnippetWithCorrectReflectedMethod()
        {
            var types = new[] { typeof(First), typeof(Second), typeof(Third) };
            var snippets = new[] { @"<member name=""M:Example.Second.SecondMethod2(System.String,System.Int32)"" />".ToNode() };
            var associations = associator.Examine(types, snippets);
            var method = Method<Second>(x => x.SecondMethod2(null, 0));

            var ass = associations.FirstOrDefault(x => x.Name == MemberName.FromMethod(method, typeof(Second))) as MethodAssociation;

            ass.ShouldNotBeNull();
            ass.Xml.ShouldEqual(snippets[0]);
            ass.Method.ShouldEqual<Second>(x => x.SecondMethod2("", 0));
        }

        [Test]
        public void ShouldAssociatePropertySnippetWithCorrectReflectedProperty()
        {
            var types = new[] { typeof(First), typeof(Second), typeof(Third) };
            var snippets = new[] { @"<member name=""P:Example.Second.SecondProperty"" />".ToNode() };
            var associations = associator.Examine(types, snippets);
            var property = Property<Second>(x => x.SecondProperty);

            var ass = associations.FirstOrDefault(x => x.Name == MemberName.FromProperty(property, typeof(Second))) as PropertyAssociation;

            ass.ShouldNotBeNull();
            ass.Xml.ShouldEqual(snippets[0]);
            ass.Property.ShouldEqual<Second>(x => x.SecondProperty);
        }

    }
}