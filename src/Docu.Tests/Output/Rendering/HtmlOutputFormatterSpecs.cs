using Docu.Documentation;
using Docu.Documentation.Comments;
using Docu.Parsing.Model;
using Docu.Output.Rendering;
using Docu.Tests.Utils;
using Example;
using Machine.Specifications;
using NUnit.Framework;

namespace Docu.Tests.Output.Rendering
{
    public class when_the_HtmlOutputFormatter_is_told_to_format_a_parameter_reference : HtmlOutputFormatterSpec
    {
        Because of = () =>
            formatted_output = formatter.Format(new ParameterReference("myParam"));

        It should_output_the_parameter_reference_in_a_var_tag = () =>
            formatted_output.ShouldEqual("<var>myParam</var>");
    }

    public class when_the_HtmlOutputFormatter_is_told_to_format_a_definition_list : HtmlOutputFormatterSpec
    {
        Because of = () =>
        {
            var list = new DefinitionList();

            list.Items.Add(listItem("a", "first"));
            list.Items.Add(listItem("b", "second"));

            formatted_output = formatter.Format(list);
        };

        It should_wrap_the_definition_list_in_a_dl_tag = () =>
        {
            formatted_output.ShouldStartWith("<dl>");
            formatted_output.ShouldEndWith("</dl>");
        };

        It should_wrap_each_item_term_in_a_dt_tag = () =>
        {
            formatted_output.ShouldContain("<dt>a</dt>");
            formatted_output.ShouldContain("<dt>b</dt>");
        };

        It should_wrap_each_item_definition_in_a_dd_tag = () =>
        {
            formatted_output.ShouldContain("<dd>first</dd>");
            formatted_output.ShouldContain("<dd>second</dd>");
        };
    }

    public class when_the_HtmlOutputFormatter_is_told_to_format_a_numbered_list : HtmlOutputFormatterSpec
    {
        Because of = () =>
        {
            var list = new NumberList();

            list.Items.Add(listItem("a"));
            list.Items.Add(listItem("b"));

            formatted_output = formatter.Format(list);
        };

        It should_wrap_the_number_list_in_a_ol_tag = () =>
        {
            formatted_output.ShouldStartWith("<ol>");
            formatted_output.ShouldEndWith("</ol>");
        };

        It should_wrap_each_item_in_a_li_tag = () =>
        {
            formatted_output.ShouldContain("<li>a</li>");
            formatted_output.ShouldContain("<li>b</li>");
        };
    }

    public class when_the_HtmlOutputFormatter_is_told_to_format_a_bullet_list : HtmlOutputFormatterSpec
    {
        Because of = () =>
        {
            var list = new BulletList();

            list.Items.Add(listItem("a"));
            list.Items.Add(listItem("b"));

            formatted_output = formatter.Format(list);
        };

        It should_wrap_the_number_list_in_a_ul_tag = () =>
        {
            formatted_output.ShouldStartWith("<ul>");
            formatted_output.ShouldEndWith("</ul>");
        };

        It should_wrap_each_item_in_a_li_tag = () =>
        {
            formatted_output.ShouldContain("<li>a</li>");
            formatted_output.ShouldContain("<li>b</li>");
        };
    }

    public class when_the_HtmlOutputFormatter_is_told_to_format_a_table : HtmlOutputFormatterSpec
    {
        Because of = () =>
        {
            var list = new TableList();

            list.Items.Add(listItem("a", "first"));
            list.Items.Add(listItem("b", "second"));

            formatted_output = formatter.Format(list);
        };

        It should_wrap_the_number_list_in_a_table_tag = () =>
        {
            formatted_output.ShouldStartWith("<table>");
            formatted_output.ShouldEndWith("</table>");
        };

        It should_wrap_each_item_term_in_a_td_tag = () =>
        {
            formatted_output.ShouldContain("<td>a</td>");
            formatted_output.ShouldContain("<td>b</td>");
        };

        It should_wrap_each_item_definition_in_a_td_tag = () =>
        {
            formatted_output.ShouldContain("<td>first</td>");
            formatted_output.ShouldContain("<td>second</td>");
        };

        It should_wrap_each_item_in_a_tr_tag = () =>
        {
            formatted_output.ShouldContain("<tr><td>a</td><td>first</td></tr>");
            formatted_output.ShouldContain("<tr><td>b</td><td>second</td></tr>");
        };
    }

    public abstract class HtmlOutputFormatterSpec
    {
        Establish context = () =>
            formatter = new HtmlOutputFormatter(Stub.Create<IDocuTemplate>());

        protected static HtmlOutputFormatter formatter;
        protected static string formatted_output;

        protected static InlineListItem listItem(string definition)
        {
            return new InlineListItem(null, paragraph(definition));
        }

        protected static InlineListItem listItem(string term, string definition)
        {
            return new InlineListItem(paragraph(term), paragraph(definition));
        }

        protected static Paragraph paragraph(string text)
        {
            return new Paragraph(new[]{new InlineText(text)});
        }
    }
}
