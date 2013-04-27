using System.Linq;
using Docu.Parsing;
using Docu.Parsing.Model;
using Example;
using NUnit.Framework;
using System.Reflection;

namespace Docu.Tests.Parsing
{
    [TestFixture]
    public class DocumentationXmlMatcherTests : BaseFixture
    {
        [SetUp]
        public void CreateAssociator()
        {
        }

        [Test]
        public void should_match_type()
        {
            var undocumentedMembers = DocumentableMemberFinder.ReflectMembersForDocumenting(new[] {typeof (First), typeof (Second), typeof (Third)});
            var snippets = new[] {@"<member name=""T:Example.Second"" />".ToNode()};
            var members = DocumentationXmlMatcher.MatchDocumentationToMembers(undocumentedMembers, snippets);

            var member = members.FirstOrDefault(x => x.Name == IdentifierFor.Type(typeof(Second))) as DocumentedType;

            member.ShouldNotBeNull();
            member.Xml.ShouldEqual(snippets[0]);
            member.TargetType.ShouldEqual(typeof (Second));
        }

        [Test]
        public void should_match_generic_type()
        {
            var undocumentedMembers = DocumentableMemberFinder.ReflectMembersForDocumenting(new[] {typeof (First), typeof (GenericDefinition<>)});
            var snippets = new[] {@"<member name=""T:Example.GenericDefinition`1"" />".ToNode()};
            var members = DocumentationXmlMatcher.MatchDocumentationToMembers(undocumentedMembers, snippets);

            var member = members.FirstOrDefault(x => x.Name == IdentifierFor.Type(typeof(GenericDefinition<>))) as DocumentedType;

            member.ShouldNotBeNull();
            member.Xml.ShouldEqual(snippets[0]);
            member.TargetType.ShouldEqual(typeof (GenericDefinition<>));
        }

        [Test]
        public void should_match_type_with_multiple_generic_arguments()
        {
            var undocumentedMembers = DocumentableMemberFinder.ReflectMembersForDocumenting(new[] {typeof (First), typeof (GenericDefinition<>), typeof (GenericDefinition<,>)});
            var snippets = new[] {@"<member name=""T:Example.GenericDefinition`2"" />".ToNode()};
            var members = DocumentationXmlMatcher.MatchDocumentationToMembers(undocumentedMembers, snippets);

            var member = members.FirstOrDefault(x => x.Name == IdentifierFor.Type(typeof(GenericDefinition<,>))) as DocumentedType;

            member.ShouldNotBeNull();
            member.Xml.ShouldEqual(snippets[0]);
            member.TargetType.ShouldEqual(typeof (GenericDefinition<,>));
        }

        [Test]
        public void should_match_method()
        {
            var undocumentedMembers = DocumentableMemberFinder.ReflectMembersForDocumenting(new[] {typeof (First), typeof (Second), typeof (Third)});
            var snippets = new[] {@"<member name=""M:Example.Second.SecondMethod"" />".ToNode()};
            var members = DocumentationXmlMatcher.MatchDocumentationToMembers(undocumentedMembers, snippets);
            var method = Method<Second>(x => x.SecondMethod());

            var member = members.FirstOrDefault(x => x.Name == IdentifierFor.Method(method, typeof(Second))) as DocumentedMethod;

            member.ShouldNotBeNull();
            member.Xml.ShouldEqual(snippets[0]);
            ((MethodInfo) member.Method).ShouldEqual<Second>(x => x.SecondMethod());
        }

        [Test]
        public void should_match_method_with_parameters()
        {
            var undocumentedMembers = DocumentableMemberFinder.ReflectMembersForDocumenting(new[] {typeof (First), typeof (Second), typeof (Third)});
            var snippets = new[] {@"<member name=""M:Example.Second.SecondMethod2(System.String,System.Int32)"" />".ToNode()};
            var members = DocumentationXmlMatcher.MatchDocumentationToMembers(undocumentedMembers, snippets);
            var method = Method<Second>(x => x.SecondMethod2(null, 0));

            var member = members.FirstOrDefault(x => x.Name == IdentifierFor.Method(method, typeof(Second))) as DocumentedMethod;

            member.ShouldNotBeNull();
            member.Xml.ShouldEqual(snippets[0]);
            ((MethodInfo) member.Method).ShouldEqual<Second>(x => x.SecondMethod2("", 0));
        }

        [Test]
        public void should_match_method_with_array_parameter()
        {
            var undocumentedMembers = DocumentableMemberFinder.ReflectMembersForDocumenting(new[] {typeof (ClassWithOverload)});
            var snippets = new[] {@"<member name=""M:Example.ClassWithOverload.MethodWithArray(System.String[])"" />".ToNode()};
            var members = DocumentationXmlMatcher.MatchDocumentationToMembers(undocumentedMembers, snippets);
            var method = Method<ClassWithOverload>(x => x.MethodWithArray(null));

            var member = members.FirstOrDefault(x => x.Name == IdentifierFor.Method(method, typeof(ClassWithOverload))) as DocumentedMethod;

            member.ShouldNotBeNull();
            member.Xml.ShouldEqual(snippets[0]);
            member.Method.ShouldEqual(method);
        }


        [Test]
        public void should_match_correct_method_overloads()
        {
            var undocumentedMembers = DocumentableMemberFinder.ReflectMembersForDocumenting(new[] {typeof (ClassWithOverload)});
            var snippets = new[] {@"<member name=""M:Example.ClassWithOverload.Method"" />".ToNode(), @"<member name=""M:Example.ClassWithOverload.Method(System.String)"" />".ToNode()};
            var members = DocumentationXmlMatcher.MatchDocumentationToMembers(undocumentedMembers, snippets);
            var method = Method<ClassWithOverload>(x => x.Method());
            var method2 = Method<ClassWithOverload>(x => x.Method(null));

            var member = members.FirstOrDefault(x => x.Name == IdentifierFor.Method(method, typeof(ClassWithOverload))) as DocumentedMethod;
            member.ShouldNotBeNull();
            member.Xml.ShouldEqual(snippets[0]);
            member.Method.ShouldEqual(method);

            var member2 = members.FirstOrDefault(x => x.Name == IdentifierFor.Method(method2, typeof(ClassWithOverload))) as DocumentedMethod;
            member2.ShouldNotBeNull();
            member2.Xml.ShouldEqual(snippets[0]);
            member2.Method.ShouldEqual(method2);
        }

        [Test]
        public void should_match_nongeneric_method_on_a_generic_type()
        {
            var undocumentedMembers = DocumentableMemberFinder.ReflectMembersForDocumenting(new[] {typeof (First), typeof (GenericDefinition<>), typeof (GenericDefinition<,>)});
            var snippets = new[] {@"<member name=""M:Example.GenericDefinition`1.AMethod"" />".ToNode()};
            var members = DocumentationXmlMatcher.MatchDocumentationToMembers(undocumentedMembers, snippets);
            var method = Method<GenericDefinition<object>>(x => x.AMethod());

            var member = members.FirstOrDefault(x => x.Name == IdentifierFor.Method(method, typeof(GenericDefinition<>))) as DocumentedMethod;

            member.ShouldNotBeNull();
            member.Xml.ShouldEqual(snippets[0]);
            member.Method.Name.ShouldEqual("AMethod");
            member.Method.IsGenericMethod.ShouldBeFalse();
        }

        [Test]
        public void should_match_generic_method_on_a_generic_type_having_a_different_generic_argument()
        {
            var undocumentedMembers = DocumentableMemberFinder.ReflectMembersForDocumenting(new[] {typeof (First), typeof (GenericDefinition<>), typeof (GenericDefinition<,>)});
            var snippets = new[] {@"<member name=""M:Example.GenericDefinition`1.BMethod``1"" />".ToNode()};
            var members = DocumentationXmlMatcher.MatchDocumentationToMembers(undocumentedMembers, snippets);
            var method = typeof (GenericDefinition<>).GetMethod("BMethod");

            var member = members.FirstOrDefault(x => x.Name == IdentifierFor.Method(method, typeof(GenericDefinition<>))) as DocumentedMethod;

            member.ShouldNotBeNull();
            member.Xml.ShouldEqual(snippets[0]);
            member.Method.ShouldBeSameAs(method);
        }

        [Test]
        public void should_match_generic_method_having_generic_parameter()
        {
            var undocumentedMembers = DocumentableMemberFinder.ReflectMembersForDocumenting(new[] {typeof (HasGenericMethods)});
            var snippets = new[] {@"<member name=""M:Example.HasGenericMethods.Do``1(``0)"" />".ToNode()};
            var members = DocumentationXmlMatcher.MatchDocumentationToMembers(undocumentedMembers, snippets);
            var method = typeof (HasGenericMethods).GetMethod("Do");

            var member = members.FirstOrDefault(x => x.Name == IdentifierFor.Method(method, typeof(HasGenericMethods))) as DocumentedMethod;

            member.ShouldNotBeNull();
            member.Xml.ShouldEqual(snippets[0]);
            member.Method.ShouldBeSameAs(method);
        }

        [Test]
        public void should_match_generic_method_having_multiple_generic_arguments_which_are_used_by_a_generic_parameter()
        {
            var undocumentedMembers = DocumentableMemberFinder.ReflectMembersForDocumenting(new[] {typeof (HasGenericMethods)});
            var snippets = new[] {@"<member name=""M:Example.HasGenericMethods.DoWithLookup``2(System.Collections.Generic.IDictionary{``0,``1},``0)"" />".ToNode()};
            var members = DocumentationXmlMatcher.MatchDocumentationToMembers(undocumentedMembers, snippets);
            var method = Method<HasGenericMethods>(x => x.DoWithLookup<string, string>(null, null));

            var member = members.FirstOrDefault(x => x.Name == IdentifierFor.Method(method, typeof(HasGenericMethods))) as DocumentedMethod;

            member.ShouldNotBeNull();
            member.Xml.ShouldEqual(snippets[0]);
            member.Method.ShouldBeSameAs(method);
        }

        [Test]
        public void should_match_generic_method_having_generic_parameter_which_has_type_defined_by_another_generic_type()
        {
            var undocumentedMembers = DocumentableMemberFinder.ReflectMembersForDocumenting(new[] {typeof (HasGenericMethods)});
            var snippets = new[] {@"<member name=""M:Example.HasGenericMethods.Evaluate``1(System.Collections.Generic.IDictionary{System.String,System.Linq.Expressions.Expression{System.Func{``0,System.Object}}},System.Int32)"" />".ToNode()};
            var members = DocumentationXmlMatcher.MatchDocumentationToMembers(undocumentedMembers, snippets);
            var method = Method<HasGenericMethods>(x => x.Evaluate<string>(null, 0));

            var member = members.FirstOrDefault(x => x.Name == IdentifierFor.Method(method, typeof(HasGenericMethods))) as DocumentedMethod;

            member.ShouldNotBeNull();
            member.Xml.ShouldEqual(snippets[0]);
            member.Method.ShouldBeSameAs(method);
        }

        [Test]
        public void should_match_property()
        {
            var undocumentedMembers = DocumentableMemberFinder.ReflectMembersForDocumenting(new[] {typeof (First), typeof (Second), typeof (Third)});
            var snippets = new[] {@"<member name=""P:Example.Second.SecondProperty"" />".ToNode()};
            var members = DocumentationXmlMatcher.MatchDocumentationToMembers(undocumentedMembers, snippets);
            var property = Property<Second>(x => x.SecondProperty);

            var member = members.FirstOrDefault(x => x.Name == IdentifierFor.Property(property, typeof(Second))) as DocumentedProperty;

            member.ShouldNotBeNull();
            member.Xml.ShouldEqual(snippets[0]);
            member.Property.ShouldEqual<Second>(x => x.SecondProperty);
        }

        [Test]
        public void should_match_event()
        {
            var undocumentedMembers = DocumentableMemberFinder.ReflectMembersForDocumenting(new[] {typeof (First), typeof (Second), typeof (Third)});
            var snippets = new[] {@"<member name=""E:Example.Second.AnEvent"" />".ToNode()};
            var members = DocumentationXmlMatcher.MatchDocumentationToMembers(undocumentedMembers, snippets);
            var ev = Event<Second>("AnEvent");

            var member = members.FirstOrDefault(x => x.Name == IdentifierFor.Event(ev, typeof(Second))) as DocumentedEvent;

            member.ShouldNotBeNull();
            member.Xml.ShouldEqual(snippets[0]);
            member.Event.ShouldEqual(ev);
        }
    }
}