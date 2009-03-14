using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using DrDoc.Associations;
using DrDoc.Generation;
using DrDoc.IO;
using DrDoc.Parsing;

namespace DrDoc
{
    public class Generic
    {
        /// <summary>
        /// test
        /// </summary>
        /// <typeparam name="C">c</typeparam>
        public void AMethod<C>()
        {
            
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Assembly.LoadFrom("NHibernate.dll");

            var transformer = new BulkTransformer(new TemplateTransformer(new HtmlGenerator(), new FileSystemOutputWriter(), new PatternTemplateResolver()));

            var parser = new DocParser(new Associator(), new AssociationTransformer(new CommentContentParser()));
            var namespaces = parser.Parse(new[] {Assembly.LoadFrom("FluentNHibernate.dll")}, File.ReadAllText("FluentNHibernate.XML"));

            transformer.TransformDirectory("templates", namespaces);

            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
