using DrDoc.Generation;
using NUnit.Framework;

namespace DrDoc.Tests
{
    [TestFixture]
    public class PatternTemplateResolverTests
    {
        [Test]
        public void MatchesSingleFilename()
        {
            var resolver = new PatternTemplateResolver();
            var namespaces = new DocNamespace[0];
            var results = resolver.Resolve("template.spark", namespaces);

            results[0].OutputPath.ShouldEqual("template.htm");
            results[0].TemplatePath.ShouldEqual("template.spark");
        }

        [Test]
        public void MatchesNamespacePatternFilename()
        {
            var resolver = new PatternTemplateResolver();
            var namespaces = new[] { new DocNamespace("One"), new DocNamespace("Two") };
            var results = resolver.Resolve("!namespace.spark", namespaces);

            results.CountShouldEqual(2);
            results[0].OutputPath.ShouldEqual("One.htm");
            results[0].TemplatePath.ShouldEqual("!namespace.spark");
            results[1].OutputPath.ShouldEqual("Two.htm");
            results[1].TemplatePath.ShouldEqual("!namespace.spark");
        }

        [Test]
        public void MatchesTypePatternFilename()
        {
            var resolver = new PatternTemplateResolver();
            var namespaces = new[] { new DocNamespace("One"), new DocNamespace("Two") };

            namespaces[0].AddType(new DocType("TypeOne"));
            namespaces[1].AddType(new DocType("TypeTwo"));

            var results = resolver.Resolve("!type.spark", namespaces);

            results.CountShouldEqual(2);
            results[0].OutputPath.ShouldEqual("One.TypeOne.htm");
            results[0].TemplatePath.ShouldEqual("!type.spark");
            results[1].OutputPath.ShouldEqual("Two.TypeTwo.htm");
            results[1].TemplatePath.ShouldEqual("!type.spark");
        }

        [Test]
        public void MatchesTemplateInDirectory()
        {
            var resolver = new PatternTemplateResolver();
            var namespaces = new DocNamespace[0];
            var results = resolver.Resolve("directory\\template.spark", namespaces);

            results.CountShouldEqual(1);
            results[0].OutputPath.ShouldEqual("directory\\template.htm");
            results[0].TemplatePath.ShouldEqual("directory\\template.spark");
        }

        [Test]
        public void MatchesPatternTemplateInDirectory()
        {
            var resolver = new PatternTemplateResolver();
            var namespaces = new[] { new DocNamespace("One"), new DocNamespace("Two") };
            var results = resolver.Resolve("directory\\!namespace.spark", namespaces);

            results.CountShouldEqual(2);
            results[0].OutputPath.ShouldEqual("directory\\One.htm");
            results[0].TemplatePath.ShouldEqual("directory\\!namespace.spark");
            results[1].OutputPath.ShouldEqual("directory\\Two.htm");
            results[1].TemplatePath.ShouldEqual("directory\\!namespace.spark");
        }

        [Test]
        public void MatchesTemplateInNamespacePatternDirectory()
        {
            var resolver = new PatternTemplateResolver();
            var namespaces = new[] { new DocNamespace("One"), new DocNamespace("Two") };
            var results = resolver.Resolve("!namespace\\template.spark", namespaces);

            results.CountShouldEqual(2);
            results[0].OutputPath.ShouldEqual("One\\template.htm");
            results[0].TemplatePath.ShouldEqual("!namespace\\template.spark");
            results[1].OutputPath.ShouldEqual("Two\\template.htm");
            results[1].TemplatePath.ShouldEqual("!namespace\\template.spark");
        }

        [Test]
        public void MatchesTemplateInTypePatternDirectory()
        {
            var resolver = new PatternTemplateResolver();
            var namespaces = new[] { new DocNamespace("One"), new DocNamespace("Two") };

            namespaces[0].AddType(new DocType("TypeOne"));
            namespaces[1].AddType(new DocType("TypeTwo"));

            var results = resolver.Resolve("!type\\template.spark", namespaces);

            results.CountShouldEqual(2);
            results[0].OutputPath.ShouldEqual("One.TypeOne\\template.htm");
            results[0].TemplatePath.ShouldEqual("!type\\template.spark");
            results[1].OutputPath.ShouldEqual("Two.TypeTwo\\template.htm");
            results[1].TemplatePath.ShouldEqual("!type\\template.spark");
        }

        [Test]
        public void MatchesTypePatternInNamespaceDirectory()
        {
            var resolver = new PatternTemplateResolver();
            var namespaces = new[] { new DocNamespace("One"), new DocNamespace("Two") };

            namespaces[0].AddType(new DocType("TypeOne"));
            namespaces[0].AddType(new DocType("TypeTwo"));
            namespaces[1].AddType(new DocType("TypeThree"));
            namespaces[1].AddType(new DocType("TypeFour"));

            var results = resolver.Resolve("!namespace\\!type.spark", namespaces);

            results.CountShouldEqual(4);
            results[0].OutputPath.ShouldEqual("One\\TypeOne.htm");
            results[0].TemplatePath.ShouldEqual("!namespace\\!type.spark");
            results[1].OutputPath.ShouldEqual("One\\TypeTwo.htm");
            results[1].TemplatePath.ShouldEqual("!namespace\\!type.spark");
            results[2].OutputPath.ShouldEqual("Two\\TypeThree.htm");
            results[2].TemplatePath.ShouldEqual("!namespace\\!type.spark");
            results[3].OutputPath.ShouldEqual("Two\\TypeFour.htm");
            results[3].TemplatePath.ShouldEqual("!namespace\\!type.spark");
        }
    }
}