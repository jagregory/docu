using Docu.Documentation;
using Docu.Output.Rendering;
using Docu.Parsing.Model;
using Docu.Tests.Utils;
using Example;
using Machine.Specifications;
using Rhino.Mocks;

namespace Docu.Tests.Output.Rendering
{
    public class when_the_HtmlOutputFormatter_is_told_to_format_a_referencable_that_s_a_type : HtmlOutputFormatterReferencableSpec
    {
        Because of = () =>
            formatted_output = formatter.FormatReferencable(Type<First>());

        It should_output_the_type_name_in_an_anchor_tag = () =>
            formatted_output.ShouldEqual("<a href=\"url\">First</a>");

        It should_generate_the_url_for_the_type = () =>
            view.AssertWasCalled(x => x.SiteResource("~/Example/First.htm"));
    }

    public class when_the_HtmlOutputFormatter_is_told_to_format_a_referencable_that_s_a_method : HtmlOutputFormatterReferencableSpec
    {
        Because of = () =>
            formatted_output = formatter.FormatReferencable(Method<Second>("SecondMethod"));

        It should_output_the_method_name_in_an_anchor_tag = () =>
            formatted_output.ShouldEqual("<a href=\"url\">SecondMethod</a>");

        It should_generate_the_url_for_the_method = () =>
            view.AssertWasCalled(x => x.SiteResource("~/Example/Second.htm#SecondMethod"));
    }

    public class when_the_HtmlOutputFormatter_is_told_to_format_a_referencable_that_s_a_property : HtmlOutputFormatterReferencableSpec
    {
        Because of = () =>
            formatted_output = formatter.FormatReferencable(Property<Second>("SecondProperty"));

        It should_output_the_method_name_in_an_anchor_tag = () =>
            formatted_output.ShouldEqual("<a href=\"url\">SecondProperty</a>");

        It should_generate_the_url_for_the_property = () =>
            view.AssertWasCalled(x => x.SiteResource("~/Example/Second.htm#SecondProperty"));
    }

    public class when_the_HtmlOutputFormatter_is_told_to_format_a_referencable_that_s_a_field : HtmlOutputFormatterReferencableSpec
    {
        Because of = () =>
            formatted_output = formatter.FormatReferencable(Field<Second>("aField"));

        It should_output_the_method_name_in_an_anchor_tag = () =>
            formatted_output.ShouldEqual("<a href=\"url\">aField</a>");

        It should_generate_the_url_for_the_property = () =>
            view.AssertWasCalled(x => x.SiteResource("~/Example/Second.htm#aField"));
    }

    public class when_the_HtmlOutputFormatter_is_told_to_format_a_referencable_that_s_an_event : HtmlOutputFormatterReferencableSpec
    {
        Because of = () =>
            formatted_output = formatter.FormatReferencable(Event<Second>("AnEvent"));

        It should_output_the_method_name_in_an_anchor_tag = () =>
            formatted_output.ShouldEqual("<a href=\"url\">AnEvent</a>");

        It should_generate_the_url_for_the_property = () =>
            view.AssertWasCalled(x => x.SiteResource("~/Example/Second.htm#AnEvent"));
    }

    public abstract class HtmlOutputFormatterReferencableSpec
    {
        Establish context = () =>
        {
            view = Mock.Create<IDocuTemplate>(mock =>
                mock.Stub(x => x.SiteResource(null))
                        .IgnoreArguments()
                        .Return("url"));

            formatter = new HtmlOutputFormatter(view);
        };

        protected static HtmlOutputFormatter formatter;
        protected static string formatted_output;
        protected static IDocuTemplate view;

        protected static DeclaredType Type<T>()
        {
            return new DeclaredType(IdentifierFor.Type(typeof(T)), Namespace.Unresolved(IdentifierFor.Namespace(typeof(T).Namespace)));
        }

        protected static Method Method<T>(string name)
        {
            return new Method(IdentifierFor.Method(typeof(T).GetMethod(name), typeof(T)), Type<T>());
        }

        protected static Property Property<T>(string name)
        {
            return new Property(IdentifierFor.Property(typeof(T).GetProperty(name), typeof(T)), Type<T>());
        }

        protected static Field Field<T>(string name)
        {
            return new Field(IdentifierFor.Field(typeof(T).GetField(name), typeof(T)), Type<T>());
        }

        protected static Event Event<T>(string name)
        {
            return new Event(IdentifierFor.Event(typeof(T).GetEvent(name), typeof(T)), Type<T>());
        }
    }
}
