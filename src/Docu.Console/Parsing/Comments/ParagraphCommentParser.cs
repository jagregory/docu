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

        public Comment Parse(ICommentParser parser, XmlNode node, bool first, bool last, ParseOptions options)
        {
            return new Paragraph(parser.Parse(node.ChildNodes));
        }
    }
}