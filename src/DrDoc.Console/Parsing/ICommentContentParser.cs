using System.Collections.Generic;
using System.Xml;
using DrDoc.Documentation.Comments;

namespace DrDoc.Parsing
{
    public interface ICommentContentParser
    {
        IList<IComment> Parse(XmlNode content);
    }
}