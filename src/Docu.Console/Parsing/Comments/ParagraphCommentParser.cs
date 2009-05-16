using System.Xml;
using Docu.Documentation.Comments;

namespace Docu.Parsing.Comments
{
    internal class ParagraphCommentParser : CommentParserBase
    {
        private readonly CommentParser parser;

        public ParagraphCommentParser(CommentParser parser)
        {
            this.parser = parser;
        }

        public IComment Parse(XmlNode content, bool first, bool last)
        {
            return new Paragraph(parser.Parse(content.ChildNodes));
        }
    }
}