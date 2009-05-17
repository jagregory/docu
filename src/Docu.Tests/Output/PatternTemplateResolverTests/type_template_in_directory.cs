using NUnit.Framework;

namespace Docu.Tests.Generation.PatternTemplateResolverTests
{
    [TestFixture]
    public class type_template_in_directory : BasePatternResolve_ViewData_Fixture
    {
        [Test]
        public void should_have_specific_namespace()
        {
            resolve("dir\\!type.spark");

            namespace_of(first_result).ShouldEqual(first_namespace);
            namespace_of(second_result).ShouldEqual(second_namespace);
            namespace_of(third_result).ShouldEqual(third_namespace);
        }

        [Test]
        public void should_have_matching_type()
        {
            resolve("dir\\!type.spark");

            type_of(first_result).ShouldEqual(first_type);
            type_of(second_result).ShouldEqual(second_type);
            type_of(third_result).ShouldEqual(third_type);
        }

        [Test]
        public void should_have_all_namespaces()
        {
            resolve("dir\\!type.spark");

            namespaces_of(first_result)
                .Count.ShouldEqual(number_of_namespaces);
            namespaces_of(first_result)
                .names_should_equal("First", "Second", "Third");
        }

        [Test]
        public void should_have_all_types()
        {
            resolve("dir\\!type.spark");

            types_of(first_result)
                .Count.ShouldEqual(number_of_types);
            types_of(first_result)
                .names_should_equal("First", "Second", "Third");
        }
    }
}