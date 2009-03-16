using System;
using Docu.Documentation;
using NUnit.Framework;

namespace Docu.Tests.UI
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
        public void should_output_lowercase_for_void()
        {
            typeof(void).GetPrettyName()
                .ShouldEqual("void");
        }

        [Test]
        public void should_output_lowercase_for_bool()
        {
            typeof(bool).GetPrettyName()
                .ShouldEqual("bool");
        }

        [Test]
        public void should_output_lowercase_for_object()
        {
            typeof(object).GetPrettyName()
                .ShouldEqual("object");
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