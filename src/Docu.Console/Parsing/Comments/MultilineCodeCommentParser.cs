using System.Xml;
using Docu.Documentation.Comments;

namespace Docu.Parsing.Comments
{
    internal class MultilineCodeCommentParser : CommentParserBase
    {
        public IComment Parse(XmlNode content)
        {
            return new InlineCode(PrepareText(content.InnerText));
        }
    }
}