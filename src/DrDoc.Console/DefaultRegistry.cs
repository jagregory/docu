using DrDoc.Generation;
using DrDoc.IO;
using StructureMap.Configuration.DSL;

namespace DrDoc
{
    internal class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            Scan(x =>
            {
                x.AssemblyContainingType<DocumentationGenerator>();
                x.WithDefaultConventions();
            });

            ForRequestedType<IOutputGenerator>()
                .TheDefault.IsThis(new HtmlGenerator());
            ForRequestedType<IOutputWriter>()
                .TheDefaultIsConcreteType<FileSystemOutputWriter>();
        }
    }
}