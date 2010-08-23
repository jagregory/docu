using System.Xml;
using Docu.Documentation.Comments;

namespace Docu.Parsing.Comments
{
    public interface ICommentNodeParser
    {
        bool CanParse(XmlNode node);
        IComment Parse(ICommentParser parser, XmlNode node, bool first, bool last, ParseOptions options);
    }
}