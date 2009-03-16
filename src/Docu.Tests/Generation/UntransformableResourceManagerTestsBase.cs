using System.IO;
using Docu.Generation;
using Docu.Generation;
using NUnit.Framework;

namespace Docu.Tests.Generation
{
    public abstract class UntransformableResourceManagerTestsBase
    {
        protected UntransformableResourceManager manager;
        protected string template_folder;
        protected string output_folder;

        [SetUp]
        public void setup()
        {
            create_dummy_folder_structure();
            create_resource_manager();
        }

        private void create_dummy_folder_structure()
        {
            destroy_dummy_folder_structure();

            var temp_path = Path.GetTempPath();

            template_folder = Path.Combine(temp_path, Path.GetRandomFileName());
            output_folder = Path.Combine(temp_path, Path.GetRandomFileName());

            Directory.CreateDirectory(template_folder);
            Directory.CreateDirectory(output_folder);

            File.WriteAllText(Path.Combine(template_folder, "a_html_page.htm"), "");
            File.WriteAllText(Path.Combine(template_folder, "a_css_file.css"), "");
            File.WriteAllText(Path.Combine(template_folder, "a_spark_file.spark"), "");

            var sub_directory = Path.Combine(template_folder, "sub_directory");

            Directory.CreateDirectory(sub_directory);

            File.WriteAllText(Path.Combine(sub_directory, "another_html_page.htm"), "");
            File.WriteAllText(Path.Combine(sub_directory, "another_spark_file.spark"), "");

            var empty_sub_directory = Path.Combine(template_folder, "empty_sub_directory");

            Directory.CreateDirectory(empty_sub_directory);
        }

        private void destroy_dummy_folder_structure()
        {
            if (Directory.Exists(template_folder))
                Directory.Delete(template_folder, true);

            if (Directory.Exists(output_folder))
                Directory.Delete(output_folder, true);
        }

        private void create_resource_manager()
        {
            manager = new UntransformableResourceManager();
        }

        protected void move_files()
        {
            manager.MoveResources(template_folder, output_folder);
        }

        private string GetPath(string path, params string[] paths)
        {
            var full_path = path;

            foreach (var p in paths)
                full_path = Path.Combine(full_path, p);

            return full_path;
        }

        protected void file_should_exist(string path, params string[] paths)
        {
            var full_path = GetPath(path, paths);

            File.Exists(full_path)
                .ShouldBeTrue();
        }

        protected void file_shouldnt_exist(string path, params string[] paths)
        {
            var full_path = GetPath(path, paths);

            File.Exists(full_path)
                .ShouldBeFalse();
        }

        protected void directory_should_exist(string path, params string[] paths)
        {
            var full_path = GetPath(path, paths);

            Directory.Exists(full_path)
                .ShouldBeTrue();
        }

        protected void directory_shouldnt_exist(string path, params string[] paths)
        {
            var full_path = GetPath(path, paths);

            Directory.Exists(full_path)
                .ShouldBeFalse();
        }
    }
}