using System.Xml;
using Docu.Documentation.Comments;

namespace Docu.Parsing.Comments
{
    public class ParagraphCommentParser : ICommentNodeParser
    {
        public bool CanParse(XmlNode node)
        {
            return node.Name == "para";
        }

        public IComment Parse(ICommentParser parser, XmlNode node, bool first, bool last)
        {
            return new Paragraph(parser.Parse(node.ChildNodes));
        }
    }
}