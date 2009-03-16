using System;
using DrDoc.Documentation;
using NUnit.Framework;

namespace DrDoc.Tests.UI
{
    [TestFixture]
    public class PrettyNameTests
    {
        [Test]
        public void should_output_just_type_name_for_non_generic_type()
        {
            typeof(DateTime).GetPrettyName()
                .ShouldEqual("DateTime");
        }

        [Test]
        public void should_output_special_type_name_for_shortcut_types()
        {
            typeof(int).GetPrettyName()
                .ShouldEqual("int");
        }

        [Test]
        public void should_output_special_type_name_for_shortcut_types_string()
        {
            typeof(string).GetPrettyName()
                .ShouldEqual("string");
        }

        [Test]
        public void should_output_name_with_generic_args_for_generic_type_definition()
        {
            typeof(Action<>).GetPrettyName()
                .ShouldEqual("Action<T>");
        }

        [Test]
        public void should_output_name_with_generic_args_for_generic_type()
        {
            typeof(Action<int>).GetPrettyName()
                .ShouldEqual("Action<int>");
        }

        [Test]
        public void should_output_name_with_generic_args_for_generic_type_of_generic_type()
        {
            typeof(Action<Func<int>>).GetPrettyName()
                .ShouldEqual("Action<Func<int>>");
        }
    }
}