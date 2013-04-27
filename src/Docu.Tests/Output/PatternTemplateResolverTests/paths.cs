using Docu.Documentation;
using Docu.Output;
using Docu.Parsing.Model;
using Example;
using NUnit.Framework;

namespace Docu.Tests.Output.PatternTemplateResolverTests
{
    [TestFixture]
    public class Paths : BaseFixture
    {
        [Test]
        public void MatchesSingleFilename()
        {
            var resolver = new PatternTemplateResolver();
            var namespaces = new Namespace[0];
            var results = resolver.Resolve("template.htm.spark", namespaces);

            results[0].OutputPath.ShouldEqual("template.htm");
            results[0].TemplatePath.ShouldEqual("template.htm.spark");
        }

        [Test]
        public void ShouldResolveDirectoriesOfSameName()
        {
            var resolver = new PatternTemplateResolver();
            var namespaces = new Namespace[0];
            var results = resolver.Resolve("dir\\dir\\template.htm.spark", namespaces);

            results[0].OutputPath.ShouldEqual("dir\\dir\\template.htm");
            results[0].TemplatePath.ShouldEqual("dir\\dir\\template.htm.spark");
        }

        [Test]
        public void MatchesNamespacePatternFilename()
        {
            var resolver = new PatternTemplateResolver();
            var namespaces = Namespaces("One", "Two");
            var results = resolver.Resolve("!namespace.htm.spark", namespaces);

            results.Count.ShouldEqual(2);
            results[0].OutputPath.ShouldEqual("One.htm");
            results[0].TemplatePath.ShouldEqual("!namespace.htm.spark");
            results[1].OutputPath.ShouldEqual("Two.htm");
            results[1].TemplatePath.ShouldEqual("!namespace.htm.spark");
        }

        [Test]
        public void MatchesTypePatternFilename()
        {
            var resolver = new PatternTemplateResolver();
            var namespaces = Namespaces("One", "Two");

            namespaces[0].AddType(new DeclaredType(IdentifierFor.Type(typeof(First)), namespaces[0]));
            namespaces[1].AddType(new DeclaredType(IdentifierFor.Type(typeof(Second)), namespaces[1]));

            var results = resolver.Resolve("!type.htm.spark", namespaces);

            results.Count.ShouldEqual(2);
            results[0].OutputPath.ShouldEqual("One.First.htm");
            results[0].TemplatePath.ShouldEqual("!type.htm.spark");
            results[1].OutputPath.ShouldEqual("Two.Second.htm");
            results[1].TemplatePath.ShouldEqual("!type.htm.spark");
        }

        [Test]
        public void MatchesTemplateInDirectory()
        {
            var resolver = new PatternTemplateResolver();
            var namespaces = new Namespace[0];
            var results = resolver.Resolve("directory\\template.htm.spark", namespaces);

            results.Count.ShouldEqual(1);
            results[0].OutputPath.ShouldEqual("directory\\template.htm");
            results[0].TemplatePath.ShouldEqual("directory\\template.htm.spark");
        }

        [Test]
        public void MatchesPatternTemplateInDirectory()
        {
            var resolver = new PatternTemplateResolver();
            var namespaces = Namespaces("One", "Two");
            var results = resolver.Resolve("directory\\!namespace.htm.spark", namespaces);

            results.Count.ShouldEqual(2);
            results[0].OutputPath.ShouldEqual("directory\\One.htm");
            results[0].TemplatePath.ShouldEqual("directory\\!namespace.htm.spark");
            results[1].OutputPath.ShouldEqual("directory\\Two.htm");
            results[1].TemplatePath.ShouldEqual("directory\\!namespace.htm.spark");
        }

        [Test]
        public void MatchesTemplateInNamespacePatternDirectory()
        {
            var resolver = new PatternTemplateResolver();
            var namespaces = Namespaces("One", "Two");
            var results = resolver.Resolve("!namespace\\template.htm.spark", namespaces);

            results.Count.ShouldEqual(2);
            results[0].OutputPath.ShouldEqual("One\\template.htm");
            results[0].TemplatePath.ShouldEqual("!namespace\\template.htm.spark");
            results[1].OutputPath.ShouldEqual("Two\\template.htm");
            results[1].TemplatePath.ShouldEqual("!namespace\\template.htm.spark");
        }

        [Test]
        public void MatchesTemplateInTypePatternDirectory()
        {
            var resolver = new PatternTemplateResolver();
            var namespaces = Namespaces("One", "Two");

            namespaces[0].AddType(new DeclaredType(IdentifierFor.Type(typeof(First)), namespaces[0]));
            namespaces[1].AddType(new DeclaredType(IdentifierFor.Type(typeof(Second)), namespaces[1]));

            var results = resolver.Resolve("!type\\template.htm.spark", namespaces);

            results.Count.ShouldEqual(2);
            results[0].OutputPath.ShouldEqual("One.First\\template.htm");
            results[0].TemplatePath.ShouldEqual("!type\\template.htm.spark");
            results[1].OutputPath.ShouldEqual("Two.Second\\template.htm");
            results[1].TemplatePath.ShouldEqual("!type\\template.htm.spark");
        }

        [Test]
        public void MatchesTypePatternInNamespaceDirectory()
        {
            var resolver = new PatternTemplateResolver();
            var namespaces = Namespaces("One", "Two");

            namespaces[0].AddType(new DeclaredType(IdentifierFor.Type(typeof(First)), namespaces[0]));
            namespaces[0].AddType(new DeclaredType(IdentifierFor.Type(typeof(Second)), namespaces[0]));
            namespaces[1].AddType(new DeclaredType(IdentifierFor.Type(typeof(First)), namespaces[1]));
            namespaces[1].AddType(new DeclaredType(IdentifierFor.Type(typeof(Second)), namespaces[1]));

            var results = resolver.Resolve("!namespace\\!type.htm.spark", namespaces);

            results.Count.ShouldEqual(4);
            results[0].OutputPath.ShouldEqual("One\\First.htm");
            results[0].TemplatePath.ShouldEqual("!namespace\\!type.htm.spark");
            results[1].OutputPath.ShouldEqual("One\\Second.htm");
            results[1].TemplatePath.ShouldEqual("!namespace\\!type.htm.spark");
            results[2].OutputPath.ShouldEqual("Two\\First.htm");
            results[2].TemplatePath.ShouldEqual("!namespace\\!type.htm.spark");
            results[3].OutputPath.ShouldEqual("Two\\Second.htm");
            results[3].TemplatePath.ShouldEqual("!namespace\\!type.htm.spark");
        }
    }
}