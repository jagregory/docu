using System;
using System.IO;
using Docu.Output.Rendering;
using Machine.Specifications;

namespace Docu.Tests.Output
{
    public class when_a_top_level_view_is_asked_to_resolve_a_path_relative_to_the_current_level : ViewPathSpec
    {
        Because of = () =>
            resolved_path = view.SiteResource("index.htm");

        It should_use_the_original_path = () =>
            resolved_path.ShouldEqual("index.htm");
    }

    public class when_a_top_level_view_is_asked_to_resolve_a_path_relative_to_the_output_root : ViewPathSpec
    {
        Because of = () =>
            resolved_path = view.SiteResource("~/index.htm");

        It should_use_the_original_path_without_the_prefix = () =>
            resolved_path.ShouldEqual("index.htm");
    }

    public class when_a_one_level_deep_view_is_asked_to_resolve_a_path_relative_to_the_current_level : ViewPathSpec
    {
        Establish context = () =>
            view.RelativeOutputPath = "sub-directory\\view.htm";

        Because of = () =>
            resolved_path = view.SiteResource("index.htm");

        It should_use_the_relative_path_without_the_prefix = () =>
            resolved_path.ShouldEqual("index.htm");
    }

    public class when_a_one_level_deep_view_is_asked_to_resolve_a_path_relative_to_the_output_root : ViewPathSpec
    {
        Establish context = () =>
            view.RelativeOutputPath = "sub-directory\\view.htm";

        Because of = () =>
            resolved_path = view.SiteResource("~/index.htm");

        It should_use_the_relative_path_without_the_prefix = () =>
            resolved_path.ShouldEqual("../index.htm");
    }

    public class when_a_two_level_deep_view_is_asked_to_resolve_a_path_relative_to_the_output_root : ViewPathSpec
    {
        Establish context = () =>
            view.RelativeOutputPath = "sub-directory\\deeper\\view.htm";

        Because of = () =>
            resolved_path = view.SiteResource("~/index.htm");

        It should_use_the_relative_path_without_the_prefix = () =>
            resolved_path.ShouldEqual("../../index.htm");
    }

    public class when_a_two_level_deep_view_is_asked_to_resolve_a_two_level_deep_path_relative_to_the_output_root_with_the_same_sub_directory : ViewPathSpec
    {
        Establish context = () =>
            view.RelativeOutputPath = "sub-directory\\deeper\\view.htm";

        Because of = () =>
            resolved_path = view.SiteResource("~/sub-directory/index.htm");

        It should_use_the_relative_path_without_the_prefix = () =>
            resolved_path.ShouldEqual("../../sub-directory/index.htm");
    }

    public class when_a_two_level_deep_view_is_asked_to_resolve_a_two_level_deep_path_relative_to_the_output_root_with_a_different_sub_directory : ViewPathSpec
    {
        Establish context = () =>
            view.RelativeOutputPath = "sub-directory\\deeper\\view.htm";

        Because of = () =>
            resolved_path = view.SiteResource("~/other-sub-directory/index.htm");

        It should_use_the_relative_path_without_the_prefix = () =>
            resolved_path.ShouldEqual("../../other-sub-directory/index.htm");
    }

    public abstract class ViewPathSpec
    {
        Establish context = () =>
            view = new TestView();

        protected static TestView view;
        protected static string resolved_path;

        protected class TestView : SparkTemplateBase
        {
            public override void Render()
            {
            }

            public override void RenderView(TextWriter writer)
            {
            }

            public override Guid GeneratedViewId
            {
                get { return Guid.NewGuid(); }
            }
        }
    }
}
