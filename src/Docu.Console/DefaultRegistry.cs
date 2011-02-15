using Docu.Console;
using Docu.Events;
using Docu.Output;
using Docu.IO;
using Docu.Parsing.Comments;
using StructureMap.Configuration.DSL;

namespace Docu
{
    internal class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            Scan(x =>
            {
                x.AssemblyContainingType<DocumentationGenerator>();
                x.WithDefaultConventions();
                x.AddAllTypesOf<ICommentNodeParser>();
            });

            For<IOutputGenerator>().Use(new HtmlGenerator());
            For<IOutputWriter>().Use<FileSystemOutputWriter>();
            For<IScreenWriter>().Use<ConsoleScreenWriter>();
            For<IScreenMessage>().Singleton();
            For<IEventAggregator>().Singleton();
        }
    }
}