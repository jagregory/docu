using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using DrDoc.Associations;
using DrDoc.Generation;
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
            var parser = new DocParser(new Associator(), new AssociationTransformer(new CommentContentParser()));
            var namespaces = parser.Parse(new[] {Assembly.LoadFrom("FluentNHibernate.dll")}, File.ReadAllText("FluentNHibernate.XML"));
            var generator = new HtmlGenerator();

            var text = generator.Convert("test.spark", namespaces);

            File.WriteAllText("output.htm", text);
            Console.ReadKey();
        }
    }
}
