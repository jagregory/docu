using System.Collections.Generic;
using System.IO;
using Docu.UI;
using Spark;
using Spark.FileSystem;

namespace Docu.Generation
{
    public class HtmlGenerator : IOutputGenerator
    {
        private readonly SparkViewEngine engine;

        public HtmlGenerator()
        {
            var setup = new SparkSettings();

            setup.AddNamespace("Docu");
            setup.AddNamespace("Docu.Parsing.Model");
            setup.AddNamespace("System.Linq");

            engine = new SparkViewEngine(setup);
            engine.ViewFolder = new FileSystemViewFolder("templates");
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

        public string Convert(string templateName, OutputData data)
        {
            string template = templateName;

            if (template.StartsWith("templates"))
                template = template.Substring(10);

            SparkViewDescriptor descriptor = new SparkViewDescriptor()
                .AddTemplate(template);
            var view = (SparkTemplateBase)engine.CreateInstance(descriptor);

            using (var writer = new StringWriter())
            {
                try
                {
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
    }
}