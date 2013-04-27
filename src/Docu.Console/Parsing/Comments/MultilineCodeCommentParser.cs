using System.Xml;
using Docu.Documentation.Comments;

namespace Docu.Parsing.Comments
{
    public class MultilineCodeCommentParser : ICommentNodeParser
    {
        public bool CanParse(XmlNode node)
        {
            return node.Name == "code";
        }

        public Comment Parse(ICommentParser parser, XmlNode node, bool first, bool last, ParseOptions options)
        {
            return new InlineCode(node.InnerText.TrimComment(true, true));
        }
    }
}