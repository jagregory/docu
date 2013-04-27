using Docu.Parsing.Model;
using NUnit.Framework;
using System.Linq;

namespace Docu.Tests.Parsing
{
    [TestFixture]
    public class IdentifierTests
    {
        [Test]
        public void should_parse_argument_list_with_single_nongeneric()
        {
            var input = "System.Int32";
            var parameterList = IdentifierFor.ExtractMethodArgumentTypes(input);
            parameterList.Count().ShouldEqual(1);
            parameterList.First().ShouldEqual("System.Int32");
        }

        [Test]
        public void should_parse_argument_list_with_multiple_nongenerics()
        {
            var input = "System.Int32,System.String";
            var parameterList = IdentifierFor.ExtractMethodArgumentTypes(input).ToArray();
            parameterList.Length.ShouldEqual(2);
            parameterList[0].ShouldEqual("System.Int32");
            parameterList[1].ShouldEqual("System.String");
        }

        [Test]
        public void should_parse_argument_list_with_multiple_generics()
        {
            var input = "Foo{``0},Bar{``1}";
            var parameterList = IdentifierFor.ExtractMethodArgumentTypes(input).ToArray();
            parameterList.Length.ShouldEqual(2);
            parameterList[0].ShouldEqual("Foo{``0}");
            parameterList[1].ShouldEqual("Bar{``1}");
        }

        [Test]
        public void should_parse_argument_list_with_generic_with_multiple_arguments()
        {
            var input = "Foo{``0,``1},Bar{``1}";
            var parameterList = IdentifierFor.ExtractMethodArgumentTypes(input).ToArray();
            parameterList.Length.ShouldEqual(2);
            parameterList[0].ShouldEqual("Foo{``0,``1}");
            parameterList[1].ShouldEqual("Bar{``1}");
        }

        [Test]
        public void should_parse_argument_list_with_generic_of_generic()
        {
            var input = "System.Linq.Expressions.Expression{System.Func{``0,System.Object}}";
            var parameterList = IdentifierFor.ExtractMethodArgumentTypes(input).ToArray();
            parameterList.Length.ShouldEqual(1);
            parameterList[0].ShouldEqual("System.Linq.Expressions.Expression{System.Func{``0,System.Object}}");
        }

        [Test]
        public void should_parse_argument_list_with_two_out_parameters()
        {
            var input = "System.Int32@,System.Double@";
            var parameterList = IdentifierFor.ExtractMethodArgumentTypes(input).ToArray();
            parameterList.Length.ShouldEqual(2);
            parameterList[0].ShouldEqual("System.Int32");
            parameterList[1].ShouldEqual("System.Double");
        }
    }
}