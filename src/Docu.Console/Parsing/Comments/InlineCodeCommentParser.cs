using System.Xml;
using Docu.Documentation.Comments;

namespace Docu.Parsing.Comments
{
    public class InlineCodeCommentParser : ICommentNodeParser
    {
        public bool CanParse(XmlNode node)
        {
            return node.Name == "c";
        }

        public IComment Parse(ICommentParser parser, XmlNode node, bool first, bool last)
        {
            return new InlineCode(node.InnerText.TrimComment(first, last));
        }
    }
}