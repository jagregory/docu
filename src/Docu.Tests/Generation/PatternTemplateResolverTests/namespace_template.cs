using NUnit.Framework;

namespace Docu.Tests.Generation.PatternTemplateResolverTests
{
    [TestFixture]
    public class namespace_template : BasePatternResolve_ViewData_Fixture
    {
        [Test]
        public void should_have_specific_namespace()
        {
            resolve("!namespace.spark");

            namespace_of(first_result).ShouldEqual(first_namespace);
            namespace_of(second_result).ShouldEqual(second_namespace);
            namespace_of(third_result).ShouldEqual(third_namespace);
        }

        [Test]
        public void should_have_no_specific_type()
        {
            resolve("!namespace.spark");

            first_result.Data.Type.ShouldBeNull();
        }

        [Test]
        public void should_have_all_namespaces()
        {
            resolve("!namespace.spark");

            namespaces_of(first_result)
                .Count.ShouldEqual(number_of_namespaces);
            namespaces_of(first_result)
                .names_should_equal("First", "Second", "Third");
        }

        [Test]
        public void should_have_all_types()
        {
            resolve("!namespace.spark");

            types_of(first_result)
                .Count.ShouldEqual(number_of_types);
            types_of(first_result)
                .names_should_equal("First", "Second", "Third");
        }
    }
}