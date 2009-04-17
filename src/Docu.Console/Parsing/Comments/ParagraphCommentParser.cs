using System;
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
            // strip paragraph if there's only one
            if (first && last)
            {
                return new InlineText(PrepareText(content.InnerText, true, true));
            }

            Paragraph paragraph = new Paragraph();

            foreach (var child in parser.Parse(content.ChildNodes))
                paragraph.AddChild(child);

            return paragraph;
        }
    }
}