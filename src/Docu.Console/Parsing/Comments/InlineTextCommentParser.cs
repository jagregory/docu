using System.Xml;
using Docu.Documentation.Comments;

namespace Docu.Parsing.Comments
{
    internal class InlineTextCommentParser : CommentParserBase
    {
        public IComment Parse(XmlNode content, bool first, bool last)
        {
            return new InlineText(PrepareText(content.InnerText, first, last));
        }
    }
}