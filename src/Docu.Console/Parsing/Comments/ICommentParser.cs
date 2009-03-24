using System.Collections.Generic;
using System.Xml;
using Docu.Documentation.Comments;

namespace Docu.Parsing.Comments
{
    public interface ICommentParser
    {
        IList<IComment> Parse(XmlNode content);
    }
}