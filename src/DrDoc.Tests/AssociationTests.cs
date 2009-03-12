using System;
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

        [Test]
        public void ShouldAssociateTypeSnippetWithCorrectReflectedType()
        {
            var types = new[] {typeof(First), typeof(Second), typeof(Third)};
            var snippets = new[] { @"<member name=""T:Example.Second"" />".ToNode()};
            var associations = associator.Examine(types, snippets);

            associations.Count.ShouldEqual(1);

            var ass = associations[0] as TypeAssociation;

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

            associations.Count.ShouldEqual(1);

            var ass = associations[0] as TypeAssociation;

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

            associations.Count.ShouldEqual(1);

            var ass = associations[0] as TypeAssociation;

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

            associations.Count.ShouldEqual(1);

            var ass = associations[0] as MethodAssociation;

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

            associations.Count.ShouldEqual(1);

            var ass = associations[0] as MethodAssociation;

            ass.ShouldNotBeNull();
            ass.Xml.ShouldEqual(snippets[0]);
            ass.Method.Name.ShouldEqual("AMethod"); //"(typeof(GenericDefinition<>).GetMethod("AMethod")))"AMethod"));
            ass.Method.IsGenericMethod.ShouldBeFalse();
        }

        [Test]
        public void ShouldAssociateGenericMethodOnGenericTypeSnippetWithCorrectReflectedMethod()
        {
            var types = new[] { typeof(First), typeof(GenericDefinition<>), typeof(GenericDefinition<,>) };
            var snippets = new[] { @"<member name=""M:Example.GenericDefinition`1.AMethod``1"" />".ToNode() };
            var associations = associator.Examine(types, snippets);

            associations.Count.ShouldEqual(1);

            var ass = associations[0] as MethodAssociation;

            ass.ShouldNotBeNull();
            ass.Xml.ShouldEqual(snippets[0]);
            ass.Method.ShouldEqual(Method.Find(typeof(GenericDefinition<>), "AMethod``1", new Type[0]));
        }

        [Test]
        public void ShouldAssociateMethodWithParametersSnippetWithCorrectReflectedMethod()
        {
            var types = new[] { typeof(First), typeof(Second), typeof(Third) };
            var snippets = new[] { @"<member name=""M:Example.Second.SecondMethod2(System.String,System.Int32)"" />".ToNode() };
            var associations = associator.Examine(types, snippets);

            associations.Count.ShouldEqual(1);

            var ass = associations[0] as MethodAssociation;

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

            associations.Count.ShouldEqual(1);

            var ass = associations[0] as PropertyAssociation;

            ass.ShouldNotBeNull();
            ass.Xml.ShouldEqual(snippets[0]);
            ass.Property.ShouldEqual<Second>(x => x.SecondProperty);
        }
    }
}