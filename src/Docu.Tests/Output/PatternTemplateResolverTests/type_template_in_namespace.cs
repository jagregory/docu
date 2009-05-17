using NUnit.Framework;

namespace Docu.Tests.Output.PatternTemplateResolverTests
{
    [TestFixture]
    public class type_template_in_namespace : BasePatternResolve_ViewData_Fixture
    {
        [Test]
        public void should_have_specific_namespace()
        {
            resolve("!namespace\\!type.spark");

            namespace_of(first_result).ShouldEqual(first_namespace);
            namespace_of(second_result).ShouldEqual(second_namespace);
            namespace_of(third_result).ShouldEqual(third_namespace);
        }

        [Test]
        public void should_have_matching_type()
        {
            resolve("!namespace\\!type.spark");

            type_of(first_result).ShouldEqual(first_type);
            type_of(second_result).ShouldEqual(second_type);
            type_of(third_result).ShouldEqual(third_type);
        }

        [Test]
        public void should_have_all_namespaces()
        {
            resolve("!namespace\\template.spark");

            namespaces_of(first_result)
                .Count.ShouldEqual(number_of_namespaces);
            namespaces_of(first_result)
                .names_should_equal("First", "Second", "Third");
        }

        [Test]
        public void should_have_all_types()
        {
            resolve("!namespace\\template.spark");

            types_of(first_result)
                .Count.ShouldEqual(number_of_types);
            types_of(first_result)
                .names_should_equal("First", "Second", "Third");
        }
    }
}