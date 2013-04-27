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

        public Comment Parse(ICommentParser parser, XmlNode node, bool first, bool last, ParseOptions options)
        {
            if (options.PreserveWhitespace)
                return new InlineText(node.InnerText.NormaliseIndent());

            return new InlineText(node.InnerText.TrimComment(first, last));
        }
    }
}