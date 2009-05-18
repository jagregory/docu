using System.Xml;
using Docu.Documentation.Comments;

namespace Docu.Parsing.Comments
{
    public class InlineTextCommentParser : ICommentNodeParser
    {
        public bool CanParse(XmlNode node)
        {
            return node is XmlText;
        }

        public IComment Parse(ICommentParser parser, XmlNode node, bool first, bool last)
        {
            return new InlineText(node.InnerText.TrimComment(first, last));
        }
    }
}