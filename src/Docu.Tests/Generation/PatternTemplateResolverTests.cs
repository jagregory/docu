using System.Collections.Generic;
using Docu.Documentation;
using Docu.Generation;
using Docu.Parsing.Model;
using Docu.Documentation;
using Docu.Generation;
using Docu.Parsing.Model;
using Example;
using NUnit.Framework;
using DeclaredType=Docu.Documentation.DeclaredType;

namespace Docu.Tests.Generation
{
    [TestFixture]
    public class PatternTemplateResolverTests : BaseFixture
    {
        [Test]
        public void MatchesSingleFilename()
        {
            var resolver = new PatternTemplateResolver();
            var assemblies = new AssemblyDoc[0];
            var results = resolver.Resolve("template.spark", assemblies);

            results[0].OutputPath.ShouldEqual("template.htm");
            results[0].TemplatePath.ShouldEqual("template.spark");
        }

        [Test]
        public void MatchesNamespacePatternFilename()
        {
            var resolver = new PatternTemplateResolver();
            var assemblies = new[] { AssemblyNamespaces("Assembly", "One", "Two") };
            var results = resolver.Resolve("!namespace.spark", assemblies);

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
            var assemblies = new[] { AssemblyNamespaces("Assembly", "One", "Two") };

            assemblies[0].Namespaces[0].AddType(new DeclaredType(Identifier.FromType(typeof(First)), assemblies[0].Namespaces[0]));
            assemblies[0].Namespaces[1].AddType(new DeclaredType(Identifier.FromType(typeof(Second)), assemblies[0].Namespaces[1]));

            var results = resolver.Resolve("!type.spark", assemblies);

            results.CountShouldEqual(2);
            results[0].OutputPath.ShouldEqual("One.First.htm");
            results[0].TemplatePath.ShouldEqual("!type.spark");
            results[1].OutputPath.ShouldEqual("Two.Second.htm");
            results[1].TemplatePath.ShouldEqual("!type.spark");
        }

        [Test]
        public void MatchesTemplateInDirectory()
        {
            var resolver = new PatternTemplateResolver();
            var assemblies = new AssemblyDoc[0];
            var results = resolver.Resolve("directory\\template.spark", assemblies);

            results.CountShouldEqual(1);
            results[0].OutputPath.ShouldEqual("directory\\template.htm");
            results[0].TemplatePath.ShouldEqual("directory\\template.spark");
        }

        [Test]
        public void MatchesPatternTemplateInDirectory()
        {
            var resolver = new PatternTemplateResolver();
            var assemblies = new[] { AssemblyNamespaces("Assembly", "One", "Two") };
            var results = resolver.Resolve("directory\\!namespace.spark", assemblies);

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
            var assemblies = new[] { AssemblyNamespaces("Assembly", "One", "Two") };
            var results = resolver.Resolve("!namespace\\template.spark", assemblies);

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
            var assemblies = new[] { AssemblyNamespaces("Assembly", "One", "Two") };

            assemblies[0].Namespaces[0].AddType(new DeclaredType(Identifier.FromType(typeof(First)), assemblies[0].Namespaces[0]));
            assemblies[0].Namespaces[1].AddType(new DeclaredType(Identifier.FromType(typeof(Second)), assemblies[0].Namespaces[1]));

            var results = resolver.Resolve("!type\\template.spark", assemblies);

            results.CountShouldEqual(2);
            results[0].OutputPath.ShouldEqual("One.First\\template.htm");
            results[0].TemplatePath.ShouldEqual("!type\\template.spark");
            results[1].OutputPath.ShouldEqual("Two.Second\\template.htm");
            results[1].TemplatePath.ShouldEqual("!type\\template.spark");
        }

        [Test]
        public void MatchesTypePatternInNamespaceDirectory()
        {
            var resolver = new PatternTemplateResolver();
            var assemblies = new[] { AssemblyNamespaces("Assembly", "One", "Two") };

            assemblies[0].Namespaces[0].AddType(new DeclaredType(Identifier.FromType(typeof(First)), assemblies[0].Namespaces[0]));
            assemblies[0].Namespaces[0].AddType(new DeclaredType(Identifier.FromType(typeof(Second)), assemblies[0].Namespaces[0]));
            assemblies[0].Namespaces[1].AddType(new DeclaredType(Identifier.FromType(typeof(First)), assemblies[0].Namespaces[1]));
            assemblies[0].Namespaces[1].AddType(new DeclaredType(Identifier.FromType(typeof(Second)), assemblies[0].Namespaces[1]));

            var results = resolver.Resolve("!namespace\\!type.spark", assemblies);

            results.CountShouldEqual(4);
            results[0].OutputPath.ShouldEqual("One\\First.htm");
            results[0].TemplatePath.ShouldEqual("!namespace\\!type.spark");
            results[1].OutputPath.ShouldEqual("One\\Second.htm");
            results[1].TemplatePath.ShouldEqual("!namespace\\!type.spark");
            results[2].OutputPath.ShouldEqual("Two\\First.htm");
            results[2].TemplatePath.ShouldEqual("!namespace\\!type.spark");
            results[3].OutputPath.ShouldEqual("Two\\Second.htm");
            results[3].TemplatePath.ShouldEqual("!namespace\\!type.spark");
        }
    }
}