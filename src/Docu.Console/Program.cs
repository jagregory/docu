using Docu.Console;
using Docu.Documentation;
using Docu.Events;
using Docu.IO;
using Docu.Output;
using Docu.Parsing;
using Docu.Parsing.Comments;

namespace Docu
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var eventAggregator = new EventAggregator();

            var commentParser = new CommentParser(new ICommentNodeParser[]
                {
                    new InlineCodeCommentParser(),
                    new InlineListCommentParser(),
                    new InlineTextCommentParser(),
                    new MultilineCodeCommentParser(),
                    new ParagraphCommentParser(),
                    new ParameterReferenceParser(),
                    new SeeCodeCommentParser(),
                });

            var documentModel = new DocumentModel(commentParser, eventAggregator);
            var pageWriter = new BulkPageWriter(new PageWriter(new HtmlGenerator(), new FileSystemOutputWriter(), new PatternTemplateResolver()));
            var generator = new DocumentationGenerator(new AssemblyLoader(), new XmlLoader(), new AssemblyXmlParser(documentModel), pageWriter, new UntransformableResourceManager(), eventAggregator);
            
            var application = new ConsoleApplication(new ConsoleScreenWriter(), generator, eventAggregator);
            application.Run(args);
        }
    }
}