using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Docu.Documentation;

namespace Docu.Output
{
    public class BulkPageWriter
    {
        private readonly IPageWriter writer;

        public BulkPageWriter(IPageWriter writer)
        {
            this.writer = writer;
        }

        public void CreatePagesFromDirectory(string templatePath, string destination, IList<Namespace> namespaces)
        {
            writer.SetTemplatePath(templatePath);

            foreach (string file in Directory.GetFiles(templatePath, "*.spark", SearchOption.AllDirectories))
            {
                if (IsPartial(file)) continue;

                writer.CreatePages(file, destination, namespaces);
            }
        }

        public void SetAssemblies(IEnumerable<Assembly> assemblies)
        {
            writer.SetAssemblies(assemblies);
        }

        private bool IsPartial(string file)
        {
            return Path.GetFileName(file).StartsWith("_");
        }
    }
}