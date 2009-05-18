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

        public IComment Parse(ICommentParser parser, XmlNode node, bool first, bool last)
        {
            return new InlineCode(node.InnerText.TrimComment(true, true));
        }
    }
}