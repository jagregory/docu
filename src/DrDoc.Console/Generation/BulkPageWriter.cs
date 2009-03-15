using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DrDoc.Documentation;

namespace DrDoc.Generation
{
    public class BulkPageWriter : IBulkPageWriter
    {
        private readonly IPageWriter writer;

        public BulkPageWriter(IPageWriter writer)
        {
            this.writer = writer;
        }

        public void CreatePagesFromDirectory(string path, string destination, IList<Namespace> namespaces)
        {
            foreach (var file in Directory.GetFiles(path, "*.spark"))
            {
                writer.CreatePages(file, destination, namespaces);
            }

            foreach (var directory in Directory.GetDirectories(path))
            {
                CreatePagesFromDirectory(directory, destination, namespaces);
            }
        }
    }
}
