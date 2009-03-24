using System.Xml;
using Docu.Documentation.Comments;

namespace Docu.Parsing.Comments
{
    internal class InlineCodeCommentParser : CommentParserBase
    {
        public IComment Parse(XmlNode content)
        {
            return new InlineCode(PrepareText(content.InnerText));
        }
    }
}