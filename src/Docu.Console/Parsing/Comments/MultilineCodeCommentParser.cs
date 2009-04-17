using System.Xml;
using Docu.Documentation.Comments;

namespace Docu.Parsing.Comments
{
    internal class MultilineCodeCommentParser : CommentParserBase
    {
        public IComment Parse(XmlNode content, bool first, bool last)
        {
            return new InlineCode(PrepareText(content.InnerText, true, true));
        }
    }
}