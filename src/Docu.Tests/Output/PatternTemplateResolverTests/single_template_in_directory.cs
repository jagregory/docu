using NUnit.Framework;

namespace Docu.Tests.Output.PatternTemplateResolverTests
{
    [TestFixture]
    public class single_template_in_directory : BasePatternResolve_ViewData_Fixture
    {
        [Test]
        public void should_have_no_specific_namespace()
        {
            resolve("dir\\template.spark");

            namespace_of(first_result).ShouldBeNull();
        }

        [Test]
        public void should_have_no_specific_type()
        {
            resolve("dir\\template.spark");

            type_of(first_result).ShouldBeNull();
        }

        [Test]
        public void should_have_all_namespaces()
        {
            resolve("dir\\template.spark");

            namespaces_of(first_result)
                .Count.ShouldEqual(number_of_namespaces);
            namespaces_of(first_result)
                .names_should_equal("First", "Second", "Third");
        }

        [Test]
        public void should_have_all_types()
        {
            resolve("dir\\template.spark");

            types_of(first_result)
                .Count.ShouldEqual(number_of_types);
            types_of(first_result)
                .names_should_equal("First", "Second", "Third");
        }
    }
}