using System.IO;
using NUnit.Framework;

namespace DrDoc.Tests.Generation
{
    [TestFixture]
    public class UntransformableResourceManagerTests : UntransformableResourceManagerTestsBase
    {
        [Test]
        public void should_move_htm_files()
        {
            move_files();

            file_should_exist(output_folder, "a_html_page.htm");
        }

        [Test]
        public void should_move_css_files()
        {
            move_files();

            file_should_exist(output_folder, "a_css_file.css");
        }

        [Test]
        public void should_move_nonempty_dirs()
        {
            move_files();

            directory_should_exist(output_folder, "sub_directory");
        }

        [Test]
        public void should_move_untransformed_files_in_nonempty_dirs()
        {
            move_files();

            file_should_exist(output_folder, "sub_directory", "another_html_page.htm");
        }

        [Test]
        public void shouldnt_move_spark_files()
        {
            move_files();

            file_shouldnt_exist(output_folder, "a_spark_file.spark");
        }

        [Test]
        public void shouldnt_move_spark_files_in_sub_dirs()
        {
            move_files();

            file_shouldnt_exist(output_folder, "sub_directory", "another_spark_file.spark");
        }

        [Test]
        public void shouldnt_move_empty_dirs()
        {
            move_files();

            directory_shouldnt_exist(output_folder, "empty_sub_directory");
        }

        [Test]
        public void should_overwrite_files()
        {
            // create css file
            File.WriteAllText(Path.Combine(output_folder, "a_css_file.css"), "");

            move_files();

            file_should_exist(output_folder, "a_css_file.css");
        }

        [Test]
        public void should_overwrite_directories()
        {
            // create css file
            Directory.CreateDirectory(Path.Combine(output_folder, "sub_directory"));

            move_files();

            directory_should_exist(output_folder, "sub_directory");
        }
    }
}