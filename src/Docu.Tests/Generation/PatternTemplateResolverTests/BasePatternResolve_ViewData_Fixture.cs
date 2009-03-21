using System;
using System.Collections.Generic;
using System.Linq;
using Docu.Documentation;
using Docu.Generation;
using Docu.Parsing.Model;
using Example;
using NUnit.Framework;

namespace Docu.Tests.Generation.PatternTemplateResolverTests
{
    public class BasePatternResolve_ViewData_Fixture : BaseFixture
    {
        private PatternTemplateResolver resolver;
        private Namespace[] namespaces;
        protected IList<TemplateMatch> results;
        protected Namespace first_namespace;
        protected Namespace second_namespace;
        protected Namespace third_namespace;
        protected DeclaredType first_type;
        protected DeclaredType second_type;
        protected DeclaredType third_type;

        [SetUp]
        public void setup()
        {
            results = null;

            create_assemblies();
            create_resolver();
        }

        private void create_resolver()
        {
            resolver = new PatternTemplateResolver();
        }

        private void create_assemblies()
        {
            first_namespace = Namespace("First");
            first_type = Type<First>(first_namespace);
            second_namespace = Namespace("Second");
            second_type = Type<Second>(second_namespace);
            third_namespace = Namespace("Third");
            third_type = Type<Third>(third_namespace);

            namespaces = new[] { first_namespace, second_namespace, third_namespace };
        }

        protected void resolve(string template)
        {
            results = resolver.Resolve(template, namespaces);
        }

        protected TemplateMatch first_result
        {
            get { return n_result(1); }
        }

        protected TemplateMatch second_result
        {
            get { return n_result(2); }
        }

        protected TemplateMatch third_result
        {
            get { return n_result(3); }
        }

        protected TemplateMatch fourth_result
        {
            get { return n_result(4); }
        }

        protected TemplateMatch fifth_result
        {
            get { return n_result(5); }
        }

        protected TemplateMatch sixth_result
        {
            get { return n_result(6); }
        }

        private TemplateMatch n_result(int n)
        {
            return results[n - 1];
        }

        protected Namespace namespace_of(TemplateMatch match)
        {
            return match.Data.Namespace;
        }

        protected Namespace namespace_of_result(int n)
        {
            return n_result(n).Data.Namespace;
        }

        protected Namespace namespace_of_results(int n)
        {
            return n_result(n).Data.Namespace;
        }

        protected IList<Namespace> namespaces_of(TemplateMatch match)
        {
            return match.Data.Namespaces;
        }

        protected IList<DeclaredType> types_of(TemplateMatch match)
        {
            return match.Data.Types;
        }

        protected DeclaredType type_of(TemplateMatch match)
        {
            return match.Data.Type;
        }

        protected DeclaredType type_of_result(int n)
        {
            return n_result(n).Data.Type;
        }

        protected int number_of_namespaces
        {
            get { return namespaces.Length; }
        }

        protected int number_of_types
        {
            get { return (from n in namespaces from t in n.Types select t).Count(); }
        }
    }

    public static class Extensions
    {
        public static void names_should_equal(this IList<Namespace> namespaces, params string[] names)
        {
            foreach (var name in names)
            {
                var s = name;
                if (namespaces.FirstOrDefault(x => x.Name == s) == null)
                    Assert.Fail("Couldn't find '" + name + "'");
            }
        }

        public static void names_should_equal(this IList<DeclaredType> types, params string[] names)
        {
            foreach (var name in names)
            {
                var s = name;
                if (types.FirstOrDefault(x => x.Name == s) == null)
                    Assert.Fail("Couldn't find '" + name + "'");
            }
        }
    }
}