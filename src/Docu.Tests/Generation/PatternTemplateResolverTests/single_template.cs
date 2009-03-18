using NUnit.Framework;

namespace Docu.Tests.Generation.PatternTemplateResolverTests
{
    [TestFixture]
    public class single_template : BasePatternResolve_ViewData_Fixture
    {
        [Test]
        public void should_have_no_specific_namespace()
        {
            resolve("template.spark");

            namespace_of(first_result).ShouldBeNull();
        }

        [Test]
        public void should_have_no_specific_type()
        {
            resolve("template.spark");

            type_of(first_result).ShouldBeNull();
        }

        [Test]
        public void should_have_all_namespaces()
        {
            resolve("template.spark");

            namespaces_of(first_result)
                .Count.ShouldEqual(number_of_namespaces);
            namespaces_of(first_result)
                .names_should_equal("First", "Second", "Third");
        }

        [Test]
        public void should_have_all_types()
        {
            resolve("template.spark");

            types_of(first_result)
                .Count.ShouldEqual(number_of_types);
            types_of(first_result)
                .names_should_equal("First", "Second", "Third");
        }
    }
}