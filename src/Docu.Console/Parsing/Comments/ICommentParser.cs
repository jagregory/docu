using System.Collections.Generic;
using System.Xml;
using Docu.Documentation.Comments;

namespace Docu.Parsing.Comments
{
    public interface ICommentParser
    {
        IList<Comment> Parse(XmlNodeList nodes);
        IList<Comment> Parse(XmlNodeList nodes, ParseOptions options);
        IList<Comment> ParseNode(XmlNode node);
        IList<Comment> ParseNode(XmlNode node, ParseOptions options);
    }
}