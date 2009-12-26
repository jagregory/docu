using System.Collections.Generic;
using System.IO;
using Docu.Output.Rendering;
using Spark;
using Spark.FileSystem;

namespace Docu.Output
{
    public class HtmlGenerator : IOutputGenerator
    {
        private readonly SparkViewEngine engine;
        private string templatePath;

        public HtmlGenerator()
        {
            var setup = new SparkSettings();

            setup.AddNamespace(typeof(Program).Namespace);
            setup.AddNamespace(typeof(TemplateExtensions.TemplateHelperExtensions).Namespace);
            setup.AddNamespace(typeof(Parsing.Model.TypeIdentifier).Namespace);
            setup.AddNamespace(typeof(System.Linq.Enumerable).Namespace);

            engine = new SparkViewEngine(setup);
            SetTemplatePath("templates");
            engine.DefaultPageBaseType = typeof(SparkTemplateBase).FullName;
        }

        public HtmlGenerator(IEnumerable<KeyValuePair<string, string>> templates)
            : this()
        {
            var viewFolder = new InMemoryViewFolder();

            foreach (var pair in templates)
            {
                viewFolder.Add(pair.Key, pair.Value);
            }

            engine.ViewFolder = viewFolder;
        }

        public string Convert(string templateName, ViewData data, string relativeOutputPath)
        {
            string template = templateName;

            if (template.StartsWith(templatePath))
                template = template.Substring(templatePath.Length + 1);

            SparkViewDescriptor descriptor = new SparkViewDescriptor()
                .AddTemplate(template);
            var view = (SparkTemplateBase)engine.CreateInstance(descriptor);

            using (var writer = new StringWriter())
            {
                try
                {
                    view.RelativeOutputPath = relativeOutputPath;
                    view.ViewData = data;
                    view.RenderView(writer);
                }
                finally
                {
                    engine.ReleaseInstance(view);
                }

                return writer.ToString();
            }
        }

        public void SetTemplatePath(string path)
        {
            templatePath = path;
            engine.ViewFolder = new FileSystemViewFolder(templatePath);
        }

    }
}