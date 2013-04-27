using System.Collections.Generic;
using Docu.Documentation;
using Docu.Output;
using Example;
using Machine.Specifications;

namespace Docu.Tests.Output
{
    public class when_the_HtmlGenerator_is_told_to_convert_a_template_that_uses_the_assemblies_collection_to_html : HtmlGeneratorSpecs
    {
        Because of = () =>
        {
            html_generator = new HtmlGenerator(template_for_content("${Assemblies[0].FullName}"));
            generated_output = html_generator.Convert(template_name, new ViewData { Assemblies = new[] { typeof(ViewData).Assembly } }, "");
        };

        It should_output_the_assembly_value = () =>
            generated_output.ShouldEqual(typeof(ViewData).Assembly.FullName);
    }

    public class when_the_HtmlGenerator_is_told_to_convert_a_template_that_uses_the_namespaces_collection_to_html : HtmlGeneratorSpecs
    {
        Because of = () =>
        {
            html_generator = new HtmlGenerator(template_for_content("${Namespaces[0].Name}"));
            generated_output = html_generator.Convert(template_name, new ViewData { Namespaces = new [] { "Example" }.to_namespaces() }, "");
        };

        It should_output_the_namespace_value = () =>
            generated_output.ShouldEqual("Example");
    }

    public class when_the_HtmlGenerator_is_told_to_convert_a_template_that_uses_the_namespace_accessor_to_html : HtmlGeneratorSpecs
    {
        Because of = () =>
        {
            html_generator = new HtmlGenerator(template_for_content("${Namespace.Name}"));
            generated_output = html_generator.Convert(template_name, new ViewData { Namespace = "Example".to_namespace() }, "");
        };

        It should_output_the_namespace_value = () =>
            generated_output.ShouldEqual("Example");
    }

    public class when_the_HtmlGenerator_is_told_to_convert_a_template_that_uses_the_namespaces_collection_with_linq_to_html : HtmlGeneratorSpecs
    {
        Because of = () =>
        {
            html_generator = new HtmlGenerator(template_for_content("<for each=\"var ns in Namespaces.Where(x => x.Name == 'Test')\">${ns.Name}</for>"));
            generated_output = html_generator.Convert(template_name, new ViewData { Namespaces = new[] { "Example", "Test" }.to_namespaces() }, "");
        };

        It should_output_the_namespace_value = () =>
            generated_output.ShouldEqual("Test");
    }

    public class when_the_HtmlGenerator_is_told_to_convert_a_template_that_uses_the_summary_of_the_type_accessor_to_html : HtmlGeneratorSpecs
    {
        Establish context = () =>
        {
            type = typeof(First).to_type();
            type.Summary.AddChild(summary_content.to_text());
        };

        Because of = () =>
        {
            html_generator = new HtmlGenerator(template_for_content("${Format(Type.Summary)}"));
            generated_output = html_generator.Convert(template_name, new ViewData { Type = type }, "");
        };

        It should_output_the_summary_content = () =>
            generated_output.ShouldEqual(summary_content);

        static DeclaredType type;
        const string summary_content = "summary";
    }

    public class when_the_HtmlGenerator_is_told_to_convert_a_template_that_uses_method_overloads_to_html : HtmlGeneratorSpecs
    {
        Establish context = () =>
        {
            type = typeof(ClassWithOverload).to_type();
            type.Methods.Add(new ClassWithOverload().method_for(x => x.Method()));
            type.Methods.Add(new ClassWithOverload().method_for(x => x.Method(null))
                                .Setup(cfg =>
                                    cfg.Parameters.Add(typeof(string).to_type().to_method_parameter_called("one"))));
        };

        Because of = () =>
        {
            html_generator = new HtmlGenerator(template_for_content("<for each=\"var method in Type.Methods\">${method.Name}(${OutputMethodParams(method)})</for>"));
            generated_output = html_generator.Convert(template_name, new ViewData { Type = type }, "");
        };

        It should_output_the_summary_content = () =>
            generated_output.ShouldEqual("Method()Method(String one)");

        static DeclaredType type;
    }

    public class when_the_HtmlGenerator_is_told_to_convert_a_template_that_uses_a_method_return_type_to_html : HtmlGeneratorSpecs
    {
        Establish context = () =>
        {
            type = typeof(ClassWithOverload).to_type();
            type.Methods.Add(new ClassWithOverload().method_for(x => x.Method())
                                .Setup(cfg =>
                                    cfg.ReturnType = typeof(string).to_type()));
        };

        Because of = () =>
        {
            html_generator = new HtmlGenerator(template_for_content("<for each=\"var method in Type.Methods\">${method.ReturnType.PrettyName}</for>"));
            generated_output = html_generator.Convert(template_name, new ViewData { Type = type }, "");
        };

        It should_output_the_summary_content = () =>
            generated_output.ShouldEqual("String");

        static DeclaredType type;
    }

    public class when_the_HtmlGenerator_is_told_to_convert_a_template_that_uses_a_property_to_html : HtmlGeneratorSpecs
    {
        Establish context = () =>
        {
            type = typeof(PropertyType).to_type();
            type.Properties.Add(new PropertyType().property_for(x => x.Property)
                                .Setup(cfg =>
                                    cfg.ReturnType = typeof(string).to_type()));
        };

        Because of = () =>
        {
            html_generator = new HtmlGenerator(template_for_content("<for each=\"var property in Type.Properties\">${property.ReturnType.PrettyName}</for>"));
            generated_output = html_generator.Convert(template_name, new ViewData { Type = type }, "");
        };

        It should_output_the_summary_content = () =>
            generated_output.ShouldEqual("String");

        static DeclaredType type;
    }

    public abstract class HtmlGeneratorSpecs
    {
        protected static HtmlGenerator html_generator;
        protected static string generated_output;
        protected const string template_name = "example";

        protected static IEnumerable<KeyValuePair<string, string>> template_for_content(string content)
        {
            return new[] { new KeyValuePair<string, string>(template_name, content) };
        }
    }
}