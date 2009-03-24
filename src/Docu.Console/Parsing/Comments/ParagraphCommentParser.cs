using System.Xml;
using Docu.Documentation.Comments;

namespace Docu.Parsing.Comments
{
    internal class ParagraphCommentParser : CommentParserBase
    {
        public IComment Parse(XmlNode content)
        {
            return new Paragraph(PrepareText(content.InnerText));
        }
    }
}