using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Spark;
using Spark.FileSystem;

namespace DrDoc.Generation
{
    public class HtmlGenerator : IOutputGenerator
    {
        private readonly SparkViewEngine engine;

        public HtmlGenerator()
        {
            engine = new SparkViewEngine();
            engine.ViewFolder = new FileSystemViewFolder(Environment.CurrentDirectory);
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
            var descriptor = new SparkViewDescriptor()
                .AddTemplate(templateName);
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
